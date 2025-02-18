using Auxiliary;
using Core.Constants;
using Core.Extensions;
using Core.Helpers;
using Core.Models;
using Core.Workers;
using NLog;

namespace Core.Managers;

internal class ConfigManager
{
    private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly ConfigWorker _configWorker = new();

    public Config Config { get; private set; } = new();

    public int GetPollingInterval() =>
        MathExt.Median(Config.PollingInterval,
                       ConfigConstants.PollingInterval_Min,
                       ConfigConstants.PollingInterval_Max);

    public int GetDeleteDelay() =>
        MathExt.Median(Config.DeleteDelay,
                       ConfigConstants.DeleteDelay_Min,
                       ConfigConstants.DeleteDelay_Max);

    public void UpdateConfigFromFile()
    {
        try
        {
            var config = _configWorker.GetConfigFromFile();
            config.Profiles.RemoveDisabled();
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

    public void UpdateConfigFileProfiles(List<ProfileInfo> profileInfos)
    {
        var profiles = ProfileHelper.GetDeepCopies(Config.Profiles);
        ProfileHelper.SetEnabledStatesFromMatchingProfileInfos(profiles, profileInfos);
        UpdateConfigFileProfiles(profiles);
        UpdateConfigFromFile();
    }

    public void UpdateConfigFileProfiles(List<Profile> profiles)
    {
        var config = new Config()
        {
            PollingInterval = Config.PollingInterval,
            DeleteDelay = Config.DeleteDelay,
            Profiles = profiles
        };
        _configWorker.UpdateConfigFile(config);
    }
}
