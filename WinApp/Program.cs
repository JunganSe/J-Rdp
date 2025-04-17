using Auxiliary;
using System.IO.Pipes;

namespace WinApp;

internal static class Program
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly Controller _controller = new();
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
            return; // Will close gracefully.
        }

        ListenForStopSignal();

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
        _mutex?.Dispose();
        _controller.DisposeTray();
    }

    private static bool IsProgramRunning()
    {
        const string mutexName = "J-Rdp.UniqueInstance";
        _mutex = new Mutex(true, mutexName, out bool isNewInstance);
        return !isNewInstance;
    }

    private static void ListenForStopSignal()
    {
        new Thread(() =>
        {
            try
            {
                using var pipeServer = new NamedPipeServerStream("J-Rdp.Stop", PipeDirection.In);
                _logger.Debug("Listening for stop signal...");
                pipeServer.WaitForConnection();
                _logger.Info("Stop signal received.");
                Application.Exit();
            }
            catch (IOException)
            {
                // Swallow IOException, which can happen if a pipe already exists.
            }
        }).Start();
    }
}