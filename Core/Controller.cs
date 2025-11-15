using Core.Commands;
using Core.Configs;
using Core.Files;
using Core.LogDisplay;
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
    private readonly ILogDisplayManager _logDisplayManager;

    private int _pollingInterval = ConfigConstants.PollingInterval_Default;
    private CancellationTokenSource? _mainLoopCancellation;
    private bool _isRunning = false;
    private bool _isStopping = false;

    public Controller(Handler_OnConfigUpdated callback_ConfigUpdated,
                      ILogDisplayManager logDisplayManager,
                      Action callback_LogClosed)
    {
        _configManager.SetCallback_ConfigUpdated(callback_ConfigUpdated);
        _logDisplayManager = logDisplayManager;
        _logDisplayManager.SetCallback_LogClosed(callback_LogClosed);
    }

    public async Task Run()
    {
        try
        {
            if (_isRunning)
            {
                _logger.Warn("Can not run controller, it is already running.");
                return;
            }

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

    public void Stop()
    {
        StopMainLoop();
        StopAndDispose();
    }

    public void ExecuteCommand(CoreCommand command)
    {
        switch (command.CommandType, command.Param)
        {
            case (CoreCommandType.OpenLogsFolder, null):
                Auxiliary.LogManager.OpenLogsFolder();
                break;

            case (CoreCommandType.OpenConfigFile, null):
                _configManager.OpenConfigFile();
                break;

            case (CoreCommandType.ShowLog, bool showLog):
                _logDisplayManager.SetVisibility(showLog);
                break;

            case (CoreCommandType.SetLogToFile, bool logToFile):
                Auxiliary.LogManager.SetFileLogging(logToFile);
                break;

            case (CoreCommandType.UpdateConfig, ConfigInfo configInfo):
                _configManager.UpdateConfig(configInfo);
                break;

            //case (CoreCommandType.SetCallback_ConfigUpdated, Handler_OnConfigUpdated callback):
            //    _configManager.SetCallback_ConfigUpdated(callback);
            //    break;

            //case (CoreCommandType.SetLogDisplayManager, ILogDisplayManager manager):
            //    _logDisplayManager = manager;
            //    break;

            //case (CoreCommandType.SetCallback_LogClosed, Action callback):
            //    _logDisplayManager.SetCallback_LogClosed(callback);
            //    break;

            default:
                _logger.Error($"Can not execute command '{command.CommandType}'. Invalid command or parameter.");
                return;
        }
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
        ApplyConfigSetting_ShowLog();
        ApplyConfigSetting_FileLogging();
        ApplyConfigSetting_PollingInterval();
        _fileManager.SetDeleteDelay(_configManager.GetDeleteDelay());
        InitializeProfiles();
    }

    private void ApplyConfigSetting_FileLogging()
    {
        bool logToFile = _configManager.Config.LogToFile;
        Auxiliary.LogManager.SetFileLogging(logToFile);
    }

    private void ApplyConfigSetting_ShowLog()
    {
        bool showLog = _configManager.Config.ShowLog;
        _logDisplayManager.SetVisibility(showLog);
    }

    private void ApplyConfigSetting_PollingInterval()
    {
        int newPollingInterval = _configManager.GetPollingInterval();
        if (newPollingInterval == _pollingInterval)
            return;

        _pollingInterval = newPollingInterval;
        _logger.Info($"Polling interval set to {_pollingInterval} ms.");
    }

    private void InitializeProfiles()
    {
        var previousProfiles = _profileManager.ProfileWrappers.Select(pw => pw.Profile).ToList();
        var enabledProfilesInConfig = _configManager.Config.Profiles.Where(p => p.Enabled).ToList();
        _profileManager.UpdateProfiles(enabledProfilesInConfig);
        _profileManager.UpdateFilesInProfileWrappers();
        _profileManager.LogProfilesSummaryIfChanged(previousProfiles);
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

    private void StopAndDispose()
    {
        if (_isStopping)
            return;

        _logger.Debug("Cleaning up...");
        _isRunning = false;
        _isStopping = true;

        // Note: _configManager, _profileManager, and _fileManager have nothing to stop or dispose.
        _configWatcherManager.StopAndDisposeConfigWatcher();
        StopMainLoop();
        _logger.Debug("Cleanup complete.");
    }

    private void StopMainLoop()
    {
        _mainLoopCancellation?.Cancel();
        _mainLoopCancellation?.Dispose();
        _mainLoopCancellation = null;
    }
}
