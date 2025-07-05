using Auxiliary;
using Core.Profiles;
using NLog;

namespace Core.Configs;

internal class ConfigManager
{
    private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly ConfigWorker _configWorker = new();
    private readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;
    private ProfileHandler? _callback_ConfigUpdated;

    public Config Config { get; private set; } = new();

    public int GetPollingInterval() =>
        MathExt.Median(Config.PollingInterval,
                       ConfigConstants.PollingInterval_Min,
                       ConfigConstants.PollingInterval_Max);

    public int GetDeleteDelay() =>
        MathExt.Median(Config.DeleteDelay,
                       ConfigConstants.DeleteDelay_Min,
                       ConfigConstants.DeleteDelay_Max);

    public void SetCallback_ConfigUpdated(ProfileHandler callback) =>
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

        var profileInfos = ProfileHelper.GetProfileInfos(Config.Profiles);
        if (_syncContext is not null)
            _syncContext.Post(_ => _callback_ConfigUpdated.Invoke(profileInfos), state: null); // Invoke on the UI thread.
        else
            _callback_ConfigUpdated.Invoke(profileInfos); // Invoke on current thread.
    }

    public void UpdateProfilesEnabledState(List<ProfileInfo> profileInfos)
    {
        var profiles = ProfileHelper.GetDeepCopies(Config.Profiles);
        ProfileHelper.SetEnabledStatesFromMatchingProfileInfos(profiles, profileInfos);
        UpdateConfigFileProfiles(profiles);
    }

    private void UpdateConfigFileProfiles(List<Profile> profiles)
    {
        var config = new Config()
        {
            PollingInterval = Config.PollingInterval,
            DeleteDelay = Config.DeleteDelay,
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
