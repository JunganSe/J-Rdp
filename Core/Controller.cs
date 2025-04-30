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
    private CancellationTokenSource? _cancellation;
    private bool _isStopping = false;

    public async Task Run()
    {
        try
        {
            Initialize();
            _cancellation = new CancellationTokenSource();

            while (true)
            {
                _cancellation.Token.ThrowIfCancellationRequested();
                // TODO: Stop MainLoop or wait until the delay?
                MainLoop();
                await Task.Delay(_pollingInterval, _cancellation.Token);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.Debug("Stopped by request.");
            StopAndDisposeAll();
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "An unexpected error occured: " + ex.Message);
            StopAndDisposeAll();
        }
    }

    public void SetCallback_ConfigUpdated(ProfileHandler callback) =>
        _configManager.SetCallback_ConfigUpdated(callback);

    public void OpenConfigFile() =>
        _configManager.OpenConfigFile();

    public void UpdateProfilesEnabledState(List<ProfileInfo> profileInfos) =>
        _configManager.UpdateProfilesEnabledState(profileInfos);

    public void Stop()
    {
        _cancellation?.Cancel();
        _cancellation?.Dispose();
        _cancellation = null;

        StopAndDisposeAll();
    }



    private void Initialize()
    {
        _configWatcherManager.StopAndDisposeConfigWatcher();
        _configWatcherManager.StartConfigWatcher(callback: InitializeConfig);
        _configManager.CreateConfigFileIfMissing();
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

    private void StopAndDisposeAll()
    {
        if (_isStopping)
            return;

        _isStopping = true;

        _configWatcherManager.StopAndDisposeConfigWatcher();
        // TODO: Implement these.
        //_configManager.StopAndDispose();
        //_profileManager.StopAndDispose();
        //_fileManager.StopAndDispose();
    }
}
