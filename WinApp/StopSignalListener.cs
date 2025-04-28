using NLog;
using System.IO.Pipes;

namespace WinApp;

internal class StopSignalListener
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;
    private CancellationTokenSource? _stopSignalListenerCancellation;

    /// <summary>
    /// Starts the stop signal listener in a new thread. <br/>
    /// Invokes the callback when a stop signal is received.
    /// </summary>
    public void Start(Action callback)
    {
        _stopSignalListenerCancellation = new CancellationTokenSource();
        Task.Run(async () =>
        {
            await WaitForStopSignal(callback, _stopSignalListenerCancellation.Token);
        });
    }

    private async Task WaitForStopSignal(Action callback, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Debug("Listening for stop signal...");
            using var pipeServer = new NamedPipeServerStream("J-Rdp.Stop", PipeDirection.In);
            await pipeServer.WaitForConnectionAsync(cancellationToken); // Throws if canceled.

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
        _stopSignalListenerCancellation?.Cancel();
        _stopSignalListenerCancellation?.Dispose();
        _stopSignalListenerCancellation = null;
    }
}
