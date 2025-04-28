using NLog;
using System.IO.Pipes;

namespace WinApp;

internal class StopSignalListener
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;
    private CancellationTokenSource? _stopSignalListernerCancellation;

    /// <summary>
    /// Starts the stop signal listener in a new thread. <br/>
    /// Invokes the callback when a stop signal is received.
    /// </summary>
    public void Start(Action callback)
    {
        _stopSignalListernerCancellation = new CancellationTokenSource();
        Task.Run(async () =>
        {
            await WaitForStopSignal(callback, _stopSignalListernerCancellation.Token);
            LogManager.Flush();
            Thread.Sleep(100); // HACK: Give some time for the log to flush, because it does not appear to block as it should.
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
        _stopSignalListernerCancellation?.Cancel();
        _stopSignalListernerCancellation?.Dispose();
        _stopSignalListernerCancellation = null;
    }
}
