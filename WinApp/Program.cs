using Auxiliary;

namespace WinApp;

internal static class Program
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly Controller _controller = new();
    private static readonly StopSignalListener _stopSignalListener = new();
    private static Mutex? _mutex;
    private static bool _isExiting;

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
            _logger.Warn("An instance is already running. Aborting...");
            StopAndCleanup();
            return;
        }

        _stopSignalListener.Start(OnStopSignalReceived);
        _controller.Run(arguments);
        Application.Run();
    }

    private static void RegisterCloseEvents()
    {
        Application.ApplicationExit += OnExit;
        AppDomain.CurrentDomain.ProcessExit += OnExit;
    }

    private static bool IsProgramRunning()
    {
        const string mutexName = "J-Rdp.UniqueInstance";
        _mutex = new Mutex(true, mutexName, out bool isNewInstance);
        return !isNewInstance;
    }

    private static void OnStopSignalReceived() => 
        Application.Exit();

    private static void OnExit(object? sender, EventArgs eventArgs)
    {
        if (_isExiting)
            return;

        _logger.Info("***** Closing application. *****");
        _isExiting = true;
        StopAndCleanup();
    }

    private static void StopAndCleanup()
    {
        _stopSignalListener.Stop();
        _controller.DisposeTray();
        _mutex?.Dispose();

        Thread.Sleep(200); // HACK: Give some time for the log to write, because LogManager Flush/Shutdown does not block as expected.
        NLog.LogManager.Shutdown();
    }
}