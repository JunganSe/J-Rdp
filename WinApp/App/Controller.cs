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



    private void Initialize(Arguments arguments)
    {
        InitializeTrayIfArgumentAllows(arguments);
        InitializeCore(arguments);
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

    private TrayCallbacks GetTrayCallbacks() => new()
    {
        ToggleConsole = _consoleManager.SetVisibility,
        OpenLogsFolder = _coreManager.OpenLogsFolder,
        OpenConfigFile = _coreManager.OpenConfigFile,
        ProfilesActiveStateChanged = _coreManager.UpdateProfilesEnabledState
    };

    private void InitializeCore(Arguments arguments)
    {
        _coreManager.Initialize();

        if (arguments.NoTray)
            return;

        _coreManager.SetCallback_ConfigUpdated((configInfo) =>
        {
            _trayManager.UpdateMenuState(configInfo);
            // TODO: - Open/close the console.
            //       - Activate/deactivate file logging.
        });
    }
}
