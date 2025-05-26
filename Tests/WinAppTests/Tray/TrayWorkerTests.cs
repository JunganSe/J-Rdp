#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

using WinApp.Tray;

namespace WinAppTests.Tray;

[TestClass]
public class TrayWorkerTests
{
    private TrayWorker _worker;

    [TestInitialize]
    public void Setup()
    {
        _worker = new TrayWorker();
    }

    [TestMethod]
    public void CreateNotifyIcon_ReturnsValid()
    {
        // Arrange

        // Act
        var trayIcon = _worker.CreateNotifyIcon();

        // Assert
        Assert.IsNotNull(trayIcon);
        Assert.IsFalse(string.IsNullOrEmpty(trayIcon.Text));
        Assert.IsNotNull(trayIcon.Icon);
        Assert.IsTrue(trayIcon.Visible);

        // Cleanup
        _worker.DisposeTray(trayIcon);
    }
}
