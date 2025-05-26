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

    [TestMethod]
    public void CreateContextMenu_NullToggleConsoleCallback_ReturnsNull()
    {
        // Arrange
        Action<bool>? callback_ToggleConsole = null;
        Action? callback_OpenConfigFile = () => { };

        // Act
        var result = _worker.CreateContextMenu(callback_ToggleConsole, callback_OpenConfigFile);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void CreateContextMenu_NullOpenConfigFileCallback_ReturnsNull()
    {
        // Arrange
        Action<bool>? callback_ToggleConsole = (bool _) => { };
        Action? callback_OpenConfigFile = null;

        // Act
        var result = _worker.CreateContextMenu(callback_ToggleConsole, callback_OpenConfigFile);

        // Assert
        Assert.IsNull(result);
    }
}
