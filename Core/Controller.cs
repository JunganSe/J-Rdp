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
    private CancellationTokenSource? _mainLoopCancellation;
    private bool _isStopping = false;

    public async Task Run()
    {
        try
        {
            _logger.Debug("Initializing...");
            Initialize();
            _mainLoopCancellation = new CancellationTokenSource();

            _logger.Debug("Running main loop...");
            await MainLoop(_mainLoopCancellation.Token); // Loops until canceled.
        }
        catch (OperationCanceledException)
        {
            _logger.Debug("Stopped by request.");
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "An unexpected error occured.");
        }
        finally
        {
            StopAndDispose();
        }
    }

    public void SetCallback_ConfigUpdated(ProfileHandler callback) =>
        _configManager.SetCallback_ConfigUpdated(callback);

    public void OpenLogsFolder() =>
        Auxiliary.LogManager.OpenLogsFolder();

    public void OpenConfigFile() =>
        _configManager.OpenConfigFile();

    public void UpdateProfilesEnabledState(List<ProfileInfo> profileInfos) =>
        _configManager.UpdateProfilesEnabledState(profileInfos);

    public void Stop()
    {
        StopMainLoop();
        StopAndDispose();
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
        _profileManager.UpdateFilesInProfileWrappers();
        _profileManager.LogProfilesSummary();
    }

    /// <summary> Loops until canceled, where it will throw OperationCanceledException. </summary>
    private async Task MainLoop(CancellationToken cancellationToken)
    {
        while (true)
        {
            _profileManager.UpdateFilesInProfileWrappers();
            _fileManager.ProcessProfileWrappers(_profileManager.ProfileWrappers);
            await Task.Delay(_pollingInterval, cancellationToken);
        }
    }

    private void StopMainLoop()
    {
        _mainLoopCancellation?.Cancel();
        _mainLoopCancellation?.Dispose();
        _mainLoopCancellation = null;
    }

    private void StopAndDispose()
    {
        if (_isStopping)
            return;

        _logger.Debug("Cleaning up...");
        _isStopping = true;

        // Note: _configManager, _profileManager, and _fileManager have nothing to stop or dispose.
        _configWatcherManager.StopAndDisposeConfigWatcher();
        StopMainLoop();
        _logger.Debug("Cleanup complete.");
    }
}
