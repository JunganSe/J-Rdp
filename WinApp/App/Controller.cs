using Auxiliary;
using Core.Configs;
using Core.Profiles;
using WinApp.CoreHandling;
using WinApp.LogConsole;
using WinApp.Tray;

namespace WinApp.App;

internal class Controller
{
    private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly ConsoleManager _consoleManager = new();
    private readonly TrayManager _trayManager = new();
    private readonly CoreManager _coreManager = new();
    private bool _isStopping = false;

    public void Start(Arguments arguments)
    {
        Initialize(arguments); // Initialize on the current thread.
        Task.Run(_coreManager.Run); // Run CoreManager asynchronously, running in parallell on the same thread.
    }

    public void Stop()
    {
        _isStopping = true;
        _consoleManager.SetVisibility(false);
        _coreManager.Stop();
        _trayManager.DisposeTray();
    }



    #region Initialization

    private void Initialize(Arguments arguments)
    {
        InitializeTrayIfArgumentAllows(arguments);
        InitializeCore(arguments);
        _consoleManager.SetCallback_ConsoleClosed(Callback_OnConsoleClosed);
    }

    private void InitializeTrayIfArgumentAllows(Arguments arguments)
    {
        if (!arguments.NoTray)
            InitializeTray();
        else
            _logger.Info("Starting without tray icon and menu.");
    }

    private void InitializeTray()
    {
        _logger.Trace("Initializing tray...");

        var trayCallbacks = GetTrayCallbacks();
        _trayManager.SetCallbacks(trayCallbacks);
        _trayManager.InitializeNotifyIconWithContextMenu();

        _logger.Debug("Tray initialized.");
    }

    private void InitializeCore(Arguments arguments)
    {
        _coreManager.Initialize();

        if (arguments.NoTray)
            return;

        _coreManager.SetCallback_ConfigUpdated(Callback_OnConfigUpdated);
    }

    #endregion

    #region Callbacks

    private TrayCallbacks GetTrayCallbacks() => new()
    {
        ToggleConsole = Callback_ToggleConsole,
        ToggleFileLogging = Callback_ToggleFileLogging,
        OpenLogsFolder = _coreManager.OpenLogsFolder,
        OpenConfigFile = _coreManager.OpenConfigFile,
        ProfilesActiveStateChanged = Callback_ProfilesActiveStateChanged
    };

    private void Callback_ToggleConsole(bool showConsole)
    {
        _consoleManager.SetVisibility(showConsole);
        
        var configInfo = new ConfigInfo() { ShowLogConsole = showConsole };
        _coreManager.UpdateConfig(configInfo);
    }

    private void Callback_OnConsoleClosed()
    {
        // Abort if stopping, to avoid updating the config when the log console closes.
        if (_isStopping)
            return;

        _trayManager.SetMenuState_ShowConsole(false);

        var configInfo = new ConfigInfo() { ShowLogConsole = false };
        _coreManager.UpdateConfig(configInfo);
    }

    private void Callback_ToggleFileLogging(bool logToFile)
    {
        LogManager.SetFileLogging(logToFile);

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
        if (configInfo.ShowLogConsole.HasValue)
            _consoleManager.SetVisibility(configInfo.ShowLogConsole.Value);

        if (configInfo.LogToFile.HasValue)
            LogManager.SetFileLogging(configInfo.LogToFile.Value);

        _trayManager.UpdateMenuState(configInfo);
    }

    #endregion
}
