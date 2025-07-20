using Auxiliary;
using Core.Configs;
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

    public void Run(Arguments arguments)
    {
        Initialize(arguments); // Initialize on the current thread.
        Task.Run(_coreManager.Run); // Run CoreManager asynchronously, running in parallell on the same thread.
    }

    public void StopCore() =>
        _coreManager.Stop();

    public void DisposeTray() =>
        _trayManager.DisposeTray();

    public void CloseAndDisposeConsole() =>
        _consoleManager.SetVisibility(false);



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
        ProfilesActiveStateChanged = _coreManager.UpdateProfilesEnabledState
    };

    private void Callback_ToggleConsole(bool showConsole)
    {
        _consoleManager.SetVisibility(showConsole);
        // TODO: Update the config file.
    }

    private void Callback_OnConsoleClosed()
    {
        _trayManager.SetMenuState_ShowConsole(false);
        // TODO: Update the config file.
    }

    private void Callback_ToggleFileLogging(bool logToFile)
    {
        LogManager.SetFileLogging(logToFile);
        // TODO: Update the config file.
    }

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
