using NLog;

namespace WinFormsApp;

internal static class Program
{
    private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    [STAThread]
    static void Main()
    {
        RegisterCloseEvents();

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
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