using NLog;
using System.IO.Pipes;

namespace WinApp;

internal class StopSignalListener
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private CancellationTokenSource? _stopSignalListernerCancellation;
    private Thread? _stopSignalListenerThread;

    public void Start()
    {
        _stopSignalListernerCancellation = new CancellationTokenSource();
        var threadStart = new ThreadStart(async () => await WaitForStopSignal(_stopSignalListernerCancellation.Token));
        _stopSignalListenerThread = new Thread(threadStart) { IsBackground = true };
        _stopSignalListenerThread.Start();
    }

    private async Task WaitForStopSignal(CancellationToken cancellationToken)
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

    public void Stop()
    {
        _stopSignalListernerCancellation?.Cancel();
        _stopSignalListenerThread?.Join(1000); // Wait for the thread to finish.
        _stopSignalListernerCancellation?.Dispose();

        Thread.Sleep(100); // To allow any pending log messages to be written.
    }
}
