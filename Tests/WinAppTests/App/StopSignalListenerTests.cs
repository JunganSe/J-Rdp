using System.IO.Pipes;
using WinApp.App;

namespace WinAppTests.App;

[TestClass]
public class StopSignalListenerTests
{
    [TestMethod]
    public void Start_ConnectsAndInvokesCallback()
    {
        // Arrange
        var listener = new StopSignalListener();
        var callbackEvent = new ManualResetEventSlim(false);
        void Callback() => callbackEvent.Set();

        // Act
        listener.Start(Callback);

        using var clientTask = Task.Run(async () =>
        {
            await Task.Delay(100);
            using var pipeClient = new NamedPipeClientStream(".", StopSignalListener.PipeName, PipeDirection.Out);
            await pipeClient.ConnectAsync(1000);
        });

        bool isCallbackInvoked = callbackEvent.Wait(2000);

        // Assert
        Assert.IsTrue(isCallbackInvoked, "Callback was not invoked when stop signal was sent.");
    }

    [TestMethod]
    public void Stop_CancelsListening_CallbackNotInvoked()
    {
        // Arrange
        var listener = new StopSignalListener();
        var callbackEvent = new ManualResetEventSlim(false);
        void Callback() => callbackEvent.Set();

        // Act
        listener.Start(Callback);
        Thread.Sleep(100);
        listener.Stop();
        bool isCallbackInvoked = callbackEvent.Wait(500);

        // Assert
        Assert.IsFalse(isCallbackInvoked, "Callback should not be invoked after Stop is called.");
    }
}
