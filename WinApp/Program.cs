using Auxiliary;
using System.IO.Pipes;

namespace WinApp;

internal static class Program
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly Controller _controller = new();
    private static Mutex? _mutex;
    private static bool _isExiting;
    private static StopSignalListener _stopSignalListener = new();

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
            return; // Will close gracefully.
        }

        _stopSignalListener.Start();

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
        if (_isExiting)
            return;

        _logger.Info("***** Closing application. *****");
        _isExiting = true;

        _stopSignalListener.Stop();

        _controller.DisposeTray();
        _mutex?.Dispose();
    }

    private static bool IsProgramRunning()
    {
        const string mutexName = "J-Rdp.UniqueInstance";
        _mutex = new Mutex(true, mutexName, out bool isNewInstance);
        return !isNewInstance;
    }
}