using Auxiliary;
using Core;
using WinApp.Tray;

namespace WinApp;

internal static class Program
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly TrayManager _trayManager = new();
    private static Mutex? _mutex; // Intentionally stored in field to keep it in memory.

    [STAThread]
    static void Main(string[] args)
    {
        RegisterCloseEvents();

        LogManager.Initialize();

        _logger.Trace("Initializing application...");
        var arguments = Arguments.Parse(args);
        LogManager.SetFileLogging(arguments.LogToFile);
        ConsoleManager.SetVisibility(arguments.ShowConsole);

        if (IsProgramRunning())
        {
            _logger.Warn("An instance of the program is already running. Closing application.");
            Environment.Exit(0);
        }

        _trayManager.InitializeNotifyIconWithContextMenu();
        _trayManager.SetMenuState_ShowConsole(arguments.ShowConsole);
        _trayManager.SetMenuState_LogToFile(arguments.LogToFile);

        _logger.Info("***** Starting application. *****");
        RunCoreInThread();
        Application.Run();
    }

    private static bool IsProgramRunning()
    {
        const string mutexName = "J-Rdp.UniqueInstance";
        _mutex = new Mutex(true, mutexName, out bool isNewInstance);
        return !isNewInstance;
    }

    private static void RegisterCloseEvents()
    {
        Application.ApplicationExit += OnExit;
        AppDomain.CurrentDomain.ProcessExit += OnExit;
    }

    private static void OnExit(object? sender, EventArgs eventArgs)
    {
        _logger.Info("***** Closing application. *****");
        _mutex?.Dispose();
        _trayManager.DisposeMenu();
    }

    private static void RunCoreInThread()
    {
        var coreThread = new Thread(() => new Controller().Run());
        coreThread.IsBackground = true;
        coreThread.Start();
    }
}