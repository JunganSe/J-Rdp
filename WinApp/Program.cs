using Auxiliary;
using System.IO.Pipes;

namespace WinApp;

internal static class Program
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly Controller _controller = new();
    private static Mutex? _mutex;
    private static bool _isExiting;
    private static CancellationTokenSource? _stopSignalListernerCancellation;
    private static Thread? _stopSignalListenerThread;

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

        StartStopSignalListener();

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

        _stopSignalListernerCancellation?.Cancel();
        _stopSignalListenerThread?.Join(1000); // Wait for the thread to finish.
        _stopSignalListernerCancellation?.Dispose();

        _controller.DisposeTray();
        _mutex?.Dispose();

        Thread.Sleep(100); // To allow any pending log messages to be written.
    }

    private static bool IsProgramRunning()
    {
        const string mutexName = "J-Rdp.UniqueInstance";
        _mutex = new Mutex(true, mutexName, out bool isNewInstance);
        return !isNewInstance;
    }

    private static void StartStopSignalListener()
    {
        _stopSignalListernerCancellation = new CancellationTokenSource();
        var threadStart = new ThreadStart(async () => await WaitForStopSignal(_stopSignalListernerCancellation.Token));
        _stopSignalListenerThread = new Thread(threadStart) { IsBackground = true };
        _stopSignalListenerThread.Start();
    }

    private static async Task WaitForStopSignal(CancellationToken cancellationToken)
    {
        try
        {
            _logger.Debug("Listening for stop signal...");
            using var pipeServer = new NamedPipeServerStream("J-Rdp.Stop", PipeDirection.In);
            await pipeServer.WaitForConnectionAsync(cancellationToken); // Throws if canceled.

            if (cancellationToken.IsCancellationRequested)
            {
                _logger.Debug("Stopped listening for stop signal.");
                return;
            }

            _logger.Info("Stop signal received.");
            Application.Exit();
        }
        catch (OperationCanceledException)
        {
            _logger.Debug("Stopped listening for stop signal.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error in stop signal listener.");
        }
    }
}