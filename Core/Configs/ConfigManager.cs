using Auxiliary;
using Core.Profiles;
using NLog;

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
        ShowLogConsole = Config.ShowLogConsole,
        LogToFile = Config.LogToFile,
        Profiles = ProfileHelper.GetProfileInfos(Config.Profiles)
    };

    public void UpdateConfig(ConfigInfo configInfo)
    {
        var profiles = ProfileHelper.GetDeepCopies(Config.Profiles);

        if (configInfo.Profiles is not null)
            ProfileHelper.SetEnabledStatesFromMatchingProfileInfos(profiles, configInfo.Profiles);

        var config = new Config()
        {
            PollingInterval = Config.PollingInterval,
            DeleteDelay = Config.DeleteDelay,
            ShowLogConsole = configInfo.ShowLogConsole ?? Config.ShowLogConsole,
            LogToFile = configInfo.LogToFile ?? Config.LogToFile,
            Profiles = profiles
        };
        _configWorker.UpdateConfigFile(config);
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
