using Auxiliary;

namespace WinApp;

internal static class Program
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly Controller _controller = new();
    private static Mutex? _mutex;

    [STAThread]
    static void Main(string[] args)
    {
        RegisterCloseEvents();
        var arguments = Arguments.Parse(args);
        LogManager.Initialize();
        LogManager.SetFileLogging(arguments.LogToFile);

        _logger.Info("***** Starting application. *****");

        if (IsProgramRunning())
        {
            _logger.Warn("An instance of the program is already running. Closing application.");
            Environment.Exit(0);
        }

        _controller.Run(arguments);
        Application.Run();
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
        _controller.DisposeTray();
    }

    private static bool IsProgramRunning()
    {
        const string mutexName = "J-Rdp.UniqueInstance";
        _mutex = new Mutex(true, mutexName, out bool isNewInstance);
        return !isNewInstance;
    }
}