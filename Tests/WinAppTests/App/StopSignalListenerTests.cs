using System.IO.Pipes;
using WinApp.App;

namespace WinAppTests.App;

[TestClass]
public class StopSignalListenerTests
{
    [TestMethod]
    [DoNotParallelize] // This test will fail if another test method that instantiates a ContextMenuStrip is being run simultaneously.
    public async Task Start_ConnectsAndInvokesCallback()
    {
        // Arrange
        var listener = new StopSignalListener();
        var callbackEvent = new ManualResetEventSlim(false);
        Action callback = callbackEvent.Set;

        // Act
        listener.Start(callback);
        await Task.Delay(200);

        _ = Task.Run(async () =>
        {
            using var pipeClient = new NamedPipeClientStream(".", StopSignalListener.PipeName, PipeDirection.Out);
            await pipeClient.ConnectAsync(2000);
        });

        bool isCallbackInvoked = callbackEvent.Wait(2500);

        // Assert
        Assert.IsTrue(isCallbackInvoked, "Callback was not invoked when stop signal was sent.");
    }

    [TestMethod]
    public void Stop_CancelsListening_CallbackNotInvoked()
    {
        // Arrange
        var listener = new StopSignalListener();
        var callbackEvent = new ManualResetEventSlim(false);
        Action callback = callbackEvent.Set;

        // Act
        listener.Start(callback);
        Thread.Sleep(100);
        listener.Stop();
        bool isCallbackInvoked = callbackEvent.Wait(500);

        // Assert
        Assert.IsFalse(isCallbackInvoked, "Callback should not be invoked after Stop is called.");
    }
}
