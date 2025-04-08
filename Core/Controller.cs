using Core.Configs;
using Core.Files;
using Core.Profiles;
using NLog;

namespace Core;

public class Controller
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly ConfigWatcherManager _configWatcherManager = new();
    private readonly ConfigManager _configManager = new();
    private readonly ProfileManager _profileManager = new();
    private readonly FileManager _fileManager = new();
    private int _pollingInterval = ConfigConstants.PollingInterval_Default;

    public void Run()
    {
        try
        {
            Initialize();

            while (true)
            {
                MainLoop();
                Thread.Sleep(_pollingInterval);
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "An unexpected error occured: " + ex.Message);
            return;
        }
    }

    public void SetCallback_ConfigUpdated(ProfileHandler callback) =>
        _configManager.SetCallback_ConfigUpdated(callback);

    public void UpdateProfilesEnabledState(List<ProfileInfo> profileInfos)
    {
        _configManager.UpdateProfilesEnabledState(profileInfos);
    }

    public void OpenConfigFile() =>
        _configManager.OpenConfigFile();



    private void Initialize()
    {
        _configWatcherManager.StopAndDisposeConfigWatcher();
        _configWatcherManager.StartConfigWatcher(callback: InitializeConfig);
        InitializeConfig();
    }

    private void InitializeConfig()
    {
        _configManager.UpdateConfigFromFile();
        _configManager.InvokeConfigUpdatedCallback();
        SetPollingInterval();
        _fileManager.SetDeleteDelay(_configManager.GetDeleteDelay());
        InitializeProfiles();
    }

    private void SetPollingInterval()
    {
        int newPollingInterval = _configManager.GetPollingInterval();
        if (newPollingInterval == _pollingInterval)
            return;

        _pollingInterval = newPollingInterval;
        _logger.Info($"Polling interval set to {_pollingInterval} ms.");
    }

    private void InitializeProfiles()
    {
        var enabledProfiles = _configManager.Config.Profiles.Where(p => p.Enabled).ToList();
        _profileManager.UpdateProfiles(enabledProfiles);
        _profileManager.UpdateFiles();
        _profileManager.LogProfilesSummary();
    }

    private void MainLoop()
    {
        _profileManager.UpdateFiles();
        _fileManager.ProcessProfileWrappers(_profileManager.ProfileWrappers);
    }
}
