using Auxiliary;
using Core.ChangesSummarizers;
using Core.Profiles;
using NLog;
using System.Linq;

namespace Core.Configs;

internal class ConfigManager
{
    private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly ConfigWorker _configWorker = new();
    private readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;
    private Handler_OnConfigUpdated? _callback_ConfigUpdated;

    public Config Config { get; private set; } = new();

    public int GetPollingInterval() =>
        MathExt.Median(Config.PollingInterval,
                       ConfigConstants.PollingInterval_Min,
                       ConfigConstants.PollingInterval_Max);

    public int GetDeleteDelay() =>
        MathExt.Median(Config.DeleteDelay,
                       ConfigConstants.DeleteDelay_Min,
                       ConfigConstants.DeleteDelay_Max);

    public void SetCallback_ConfigUpdated(Handler_OnConfigUpdated callback) =>
        _callback_ConfigUpdated = callback;

    public void CreateConfigFileIfMissing()
    {
        if (_configWorker.IsConfigFileFound())
            return;

        _logger.Warn("Config file not found.");
        _configWorker.CreateConfigFile();
    }

    public void UpdateConfigFromFile()
    {
        try
        {
            var config = _configWorker.GetConfigFromFile();
            LogInvalidProfiles(config.Profiles);
            config.Profiles.RemoveInvalid();
            config.Profiles.AddDefaultFilterFileEndings();
            Config = config;
        }
        catch
        {
            Config = new();
            _logger.Warn("Failed to update config. Reverting to default configuration.");
        }
    }

    private void LogInvalidProfiles(List<Profile> profiles)
    {
        foreach (var profile in profiles)
        {
            if (!profile.IsValid(out string reason))
                _logger.Warn($"Profile '{profile.Name}' is invalid and will be ignored. Reason: {reason}");
        }
    }

    public void LogConfigChanges(Config oldConfig)
    {
        // TODO: Refactor this method into ConfigWorker.
        List<string> configChanges = ConfigChangesSummarizer.GetChangedConfigSettings(oldConfig, Config);
        List<string> profileChanges = ProfileChangesSummarizer.GetChangedProfilesSettings(oldConfig.Profiles, Config.Profiles);
        
        int totalChangesCount = configChanges.Count + profileChanges.Count;
        if (totalChangesCount == 0)
        {
            _logger.Info("No config settings were changed.");
            return;
        }

        var output = new List<string>();

        if (configChanges.Count > 0)
        {
            string lines = string.Join("\n", configChanges.Select(s => $"  {s}"));
            _logger.Info("Changed config settings:\n" + lines);
        }

        if (profileChanges.Count > 0)
        {
            string lines = string.Join("\n", profileChanges.Select(s => $"  {s}"));
            _logger.Info("Changed profile settings:\n" + lines);
        }
    }

    public void LogFullConfig()
    {
        string jsonConfig = _configWorker.SerializeConfig(Config);
        _logger.Trace(jsonConfig);
    }

    public void InvokeConfigUpdatedCallback()
    {
        if (_callback_ConfigUpdated is null)
            return;

        var configInfo = GetConfigInfo();
        if (_syncContext is not null)
            _syncContext.Post(_ => _callback_ConfigUpdated.Invoke(configInfo), state: null); // Invoke on the UI thread.
        else
            _callback_ConfigUpdated.Invoke(configInfo); // Invoke on current thread.
    }

    private ConfigInfo GetConfigInfo() => new()
    {
        ShowLog = Config.ShowLog,
        LogToFile = Config.LogToFile,
        Profiles = ProfileHelper.GetProfileInfos(Config.Profiles)
    };

    public void UpdateConfig(ConfigInfo configInfo)
    {
        var config = new Config()
        {
            PollingInterval = Config.PollingInterval,
            DeleteDelay = Config.DeleteDelay,
            ShowLog = configInfo.ShowLog ?? Config.ShowLog,
            LogToFile = configInfo.LogToFile ?? Config.LogToFile,
            Profiles = GetProfilesForConfigUpdate(configInfo.Profiles)
        };
        _configWorker.UpdateConfigFile(config);
    }

    private List<Profile> GetProfilesForConfigUpdate(List<ProfileInfo>? profileInfos)
    {
        if (profileInfos is null)
            return Config.Profiles;

        var profiles = ProfileHelper.GetDeepCopies(Config.Profiles);
        ProfileHelper.SetEnabledStatesFromMatchingProfileInfos(profiles, profileInfos);
        return profiles;
    }

    public void OpenConfigFile()
    {
        if (!_configWorker.IsConfigFileFound())
        {
            _logger.Error($"Failed to open config file. File not found.");
            _configWorker.CreateConfigFile();
        }

        _configWorker.OpenConfigFile();
    }
}
