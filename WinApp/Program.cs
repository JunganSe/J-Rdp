#pragma warning disable IDE0052 // Remove unread private members

using Core;
using NLog;
using WinApp.Tray;

namespace WinApp;

internal static class Program
{
    private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private static Mutex? _mutex; // Intentionally stored in field to keep it in memory.

    internal static NotifyIcon? NotifyIcon;

    [STAThread]
    static void Main(string[] args)
    {
        RegisterCloseEvents();

        var logManager = new Auxiliary.LogManager();
        logManager.Initialize();

        _logger.Trace("Initializing application...");
        var arguments = Arguments.Parse(args);
        logManager.SetFileLogging(arguments.LogToFile);
        ConsoleManager.SetVisibility(arguments.ShowConsole);

        if (IsProgramRunning())
        {
            _logger.Warn("An instance of the program is already running. Closing application.");
            Environment.Exit(0);
        }

        NotifyIcon = TrayManager.GetNotifyIcon();

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
        NotifyIcon?.ContextMenuStrip?.Dispose();
        NotifyIcon?.Dispose();
    }

    private static void RunCoreInThread()
    {
        var coreThread = new Thread(() =>
        {
            new Controller().Run();
        });
        coreThread.IsBackground = true;
        coreThread.Start();
    }
}