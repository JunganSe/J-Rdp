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
        var callbackInvoked = new ManualResetEventSlim(false);
        void Callback() => callbackInvoked.Set();

        // Act
        listener.Start(Callback);

        using var clientTask = Task.Run(async () =>
        {
            await Task.Delay(100);
            string pipeName = StopSignalListener.PipeName;
            using var client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
            await client.ConnectAsync(1000);
        });

        // Assert
        bool signalWasSet = callbackInvoked.Wait(2000);
        Assert.IsTrue(signalWasSet, "Callback was not invoked when stop signal was sent.");
    }
}
