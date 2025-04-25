using NLog;
using System.IO.Pipes;

namespace WinApp;

internal class StopSignalListener
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;
    private CancellationTokenSource? _stopSignalListernerCancellation;
    private Thread? _stopSignalListenerThread;

    /// <summary>
    /// Starts the stop signal listener in a new thread. <br/>
    /// Invokes the callback when a stop signal is received.
    /// </summary>
    public void Start(Action callback)
    {
        _stopSignalListernerCancellation = new CancellationTokenSource();
        var threadStart = new ThreadStart(async () => await WaitForStopSignal(callback, _stopSignalListernerCancellation.Token));
        _stopSignalListenerThread = new Thread(threadStart) { IsBackground = true };
        _stopSignalListenerThread.Start();
    }

    private async Task WaitForStopSignal(Action callback, CancellationToken cancellationToken)
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
            InvokeCallback(callback);
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

    private void InvokeCallback(Action callback)
    {
        if (_syncContext is not null)
            _syncContext.Post(_ => callback.Invoke(), null); // Invoke on the calling thread.
        else
            callback.Invoke(); // Invoke on the listener thread.
    }

    public void Stop()
    {
        _stopSignalListernerCancellation?.Cancel();
        _stopSignalListenerThread?.Join(1000); // Wait for the thread to finish.
        _stopSignalListernerCancellation?.Dispose();

        Thread.Sleep(100); // To allow any pending log messages to be written.
    }
}
