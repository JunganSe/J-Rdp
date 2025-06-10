#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable IDE0039 // Use local function
#pragma warning disable IDE0350 // Use implicitly typed lambda

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
    public void CreateContextMenu_NullToggleConsoleCallback_ReturnsNull()
    {
        // Arrange
        Action<bool>? callback_ToggleConsole = null;
        Action? callback_OpenLogsFolder = () => { };
        Action? callback_OpenConfigFile = () => { };

        // Act
        var result = _worker.CreateContextMenu(callback_ToggleConsole, callback_OpenLogsFolder, callback_OpenConfigFile);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void CreateContextMenu_NullOpenConfigFileCallback_ReturnsNull()
    {
        // Arrange
        Action<bool>? callback_ToggleConsole = (bool _) => { };
        Action? callback_OpenLogsFolder = null;
        Action? callback_OpenConfigFile = null;

        // Act
        var result = _worker.CreateContextMenu(callback_ToggleConsole, callback_OpenLogsFolder, callback_OpenConfigFile);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void CreateContextMenu_ValidCallbacks_ReturnsValidMenu()
    {
        // Arrange
        Action<bool>? callback_ToggleConsole = (bool _) => { };
        Action? callback_OpenLogsFolder = () => { };
        Action? callback_OpenConfigFile = () => { };

        // Act
        var menu = _worker.CreateContextMenu(callback_ToggleConsole, callback_OpenLogsFolder, callback_OpenConfigFile);

        // Assert
        Assert.IsNotNull(menu);
        Assert.IsTrue(menu.Items.Count > 0);

        // Cleanup
        menu.Dispose();
    }
}
