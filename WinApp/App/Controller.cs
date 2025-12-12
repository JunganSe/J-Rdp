using Core;
using Core.Configs;
using Core.Profiles;
using WinApp.CoreHandling;
using WinApp.LogConsole;
using WinApp.Tray;

namespace WinApp.App;

internal class Controller
{
    private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly TrayManager _trayManager = new();
    private readonly CoreManager _coreManager = new();
    private bool _isStopping = false;

    public void Start()
    {
        InitializeTray();
        InitializeCore(); // Initialize on the current thread.
        Task.Factory.StartNew(_coreManager.Run, TaskCreationOptions.LongRunning); // Runs asynchronously on a separate thread, handled by the task scheduler.
    }

    public void Stop()
    {
        _isStopping = true;
        _coreManager.ShowLog(false);
        _coreManager.Stop();
        _trayManager.DisposeTray();
    }



    #region Initialization

    private void InitializeTray()
    {
        _logger.Trace("Initializing tray...");

        var trayCallbacks = GetTrayCallbacks();
        _trayManager.SetCallbacks(trayCallbacks);
        _trayManager.InitializeNotifyIconWithContextMenu();

        _logger.Debug("Tray initialized.");
    }


    private void InitializeCore()
    {
        var coreControllerInitParams = new ControllerInitParams(
                Callback_ConfigUpdated: Callback_OnConfigUpdated,
                LogDisplayManager: new LogConsoleManager(),
                Callback_LogClosed: Callback_OnLogDisplayClosed);
        _coreManager.Initialize(coreControllerInitParams);
    }

    #endregion

    #region Callbacks

    private TrayCallbacks GetTrayCallbacks() => new()
    {
        ToggleConsole = Callback_ToggleLogDisplay,
        ToggleFileLogging = Callback_ToggleFileLogging,
        OpenLogsFolder = _coreManager.OpenLogsFolder,
        OpenConfigFile = _coreManager.OpenConfigFile,
        ProfilesActiveStateChanged = Callback_ProfilesActiveStateChanged
    };

    private void Callback_ToggleLogDisplay(bool showLog)
    {
        _coreManager.ShowLog(showLog);
        // UpdateConfig is called from Callback_OnLogDisplayClosed, which triggers when the log window is closed.
    }

    private void Callback_OnLogDisplayClosed()
    {
        // Abort if stopping, to avoid updating the config when the log console closes.
        if (_isStopping)
            return;

        _trayManager.SetMenuState_ShowConsole(false);

        var configInfo = new ConfigInfo() { ShowLog = false };
        _coreManager.UpdateConfig(configInfo);
    }

    private void Callback_ToggleFileLogging(bool logToFile)
    {
        _coreManager.SetLogToFile(logToFile);

        var configInfo = new ConfigInfo() { LogToFile = logToFile };
        _coreManager.UpdateConfig(configInfo);
    }

    private void Callback_ProfilesActiveStateChanged(List<ProfileInfo> profileInfos)
    {
        var configInfo = new ConfigInfo() { Profiles = profileInfos };
        _coreManager.UpdateConfig(configInfo);
    }

    /// <summary>
    /// Updates the state in WinApp when the config has been updated in Core.
    /// </summary>
    private void Callback_OnConfigUpdated(ConfigInfo configInfo)
    {
        _trayManager.UpdateMenuState(configInfo);
    }

    #endregion
}
