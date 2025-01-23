#pragma warning disable IDE0052 // Remove unread private members

using NLog;

namespace WinFormsApp;

internal static class Program
{
    private static Mutex? _mutex; // Intentionally stored in field to keep it in memory.
    private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    [STAThread]
    static void Main()
    {
        RegisterCloseEvents();

        if (IsProgramRunning())
        {
            _logger.Warn("An instance of the program is already running. Closing application.");
            Environment.Exit(0);
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }

    private static bool IsProgramRunning()
    {
        const string mutexName = "J-Rdp.UniqueInstance";
        _mutex = new Mutex(true, mutexName, out bool isNewInstance);
        return !isNewInstance;
    }

    private static void RegisterCloseEvents()
    {
        Application.ApplicationExit += OnClose;
        AppDomain.CurrentDomain.ProcessExit += OnClose;
    }

    private static void OnClose(object? sender, EventArgs eventArgs)
    {
        _logger.Info("Closing application by request.");
    }
}