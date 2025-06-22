#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

using WinApp.Tray;

namespace WinAppTests.Tray;

[TestClass]
public class TrayWorkerTests_Creation
{
    private TrayWorker _worker;

    [TestInitialize]
    public void InitializeTest()
    {
        _worker = new TrayWorker();
    }

    [TestMethod]
    public void CreateNotifyIcon_ReturnsValidIcon()
    {
        // Arrange

        // Act
        var trayIcon = _worker.CreateNotifyIcon();

        // Assert
        Assert.IsNotNull(trayIcon);
        Assert.IsNotNull(trayIcon.Icon);
        Assert.IsFalse(string.IsNullOrEmpty(trayIcon.Text));
        Assert.IsTrue(trayIcon.Visible);

        // Cleanup
        _worker.DisposeTray(trayIcon);
    }

    [TestMethod]
    public void CreateContextMenu_ValidCallbacks_ReturnsValidMenu()
    {
        // Arrange
        var callbacks = new TrayCallbacks();

        // Act
        var menu = _worker.CreateContextMenu(callbacks);

        // Assert
        Assert.IsNotNull(menu);
        Assert.IsTrue(menu.Items.Count > 0);

        // Cleanup
        menu.Dispose();
    }
}
