#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable IDE0039 // Use local function
#pragma warning disable IDE0350 // Use implicitly typed lambda

using Core.Profiles;
using System.Windows.Forms;
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

    #region Creation

    [TestMethod]
    public void CreateNotifyIcon_ReturnsValid()
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

    [TestMethod]
    public void CreateContextMenu_ValidCallbacks_ReturnsContextMenuStrip()
    {
        // Arrange
        Action<bool>? callback_ToggleConsole = (bool _) => { };
        Action? callback_OpenConfigFile = () => { };

        // Act
        var menu = _worker.CreateContextMenu(callback_ToggleConsole, callback_OpenConfigFile);

        // Assert
        Assert.IsNotNull(menu);
        Assert.IsTrue(menu.Items.Count > 0);

        // Cleanup
        DisposeMenu(menu);
    }

    #endregion

    #region Menu checked state

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, true)]
    [DataRow(true, false)]
    public void SetMenuCheckedState_SingleItem_SetsCheckedState(bool initialState, bool targetState)
    {
        // Arrange
        var menu = new ContextMenuStrip();
        var item = new ToolStripMenuItem { Name = "TestItem", Checked = initialState };
        menu.Items.Add(item);

        // Act
        _worker.SetMenuCheckedState(menu, "TestItem", targetState);

        // Assert
        Assert.AreEqual(targetState, item.Checked);

        // Cleanup
        DisposeMenu(menu);
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, true)]
    [DataRow(true, false)]
    public void SetMenuCheckedState_MultipleItems_SetsCheckedState(bool initialState, bool targetState)
    {
        // Arrange
        var menu = new ContextMenuStrip();
        var items = new ToolStripMenuItem[]
        {
            new() { Name = "DummyItem1", Checked = !targetState },
            new() { Name = "TestItem1", Checked = initialState },
            new() { Name = "DummyItem2", Checked = !targetState },
            new() { Name = "TestItem2", Checked = initialState },
        };
        menu.Items.AddRange(items);

        // Act
        _worker.SetMenuCheckedState(menu, "TestItem1", targetState);
        _worker.SetMenuCheckedState(menu, "TestItem2", targetState);

        // Assert
        Assert.AreEqual(targetState, items.First(i => i.Name == "TestItem1").Checked);
        Assert.AreEqual(targetState, items.First(i => i.Name == "TestItem2").Checked);
        Assert.AreNotEqual(targetState, items.First(i => i.Name == "DummyItem1").Checked);
        Assert.AreNotEqual(targetState, items.First(i => i.Name == "DummyItem2").Checked);

        // Cleanup
        DisposeMenu(menu);
    }

    #endregion

    #region Menu items

    [TestMethod]
    public void RemoveAllProfileMenuItems_RemovesOnlyProfileItems()
    {
        // Arrange
        var menu = new ContextMenuStrip();
        string prefix = TrayConstants.ItemNames.ProfilePrefix;
        var profileItem1 = new ToolStripMenuItem { Name = $"{prefix}_1" };
        var profileItem2 = new ToolStripMenuItem { Name = $"{prefix}_2" };
        var otherItem = new ToolStripMenuItem { Name = "OtherItem" };
        menu.Items.Add(profileItem1);
        menu.Items.Add(profileItem2);
        menu.Items.Add(otherItem);

        // Act
        _worker.RemoveAllProfileMenuItems(menu.Items);

        // Assert
        Assert.IsFalse(menu.Items.Contains(profileItem1));
        Assert.IsFalse(menu.Items.Contains(profileItem2));
        Assert.IsTrue(menu.Items.Contains(otherItem));

        // Cleanup
        DisposeMenu(menu);
    }

    [TestMethod]
    public void InsertProfileMenuItems_InsertsItemsAtInsertPoint()
    {
        // Arrange
        var menu = new ContextMenuStrip();
        var menuItems = new ToolStripItem[]
        {
            new ToolStripMenuItem() { Name = "Dummy Item 1" },
            new ToolStripSeparator() { Name = TrayConstants.ItemNames.ProfilesInsertPoint },
            new ToolStripMenuItem() { Name = "Dummy Item 2" },
        };
        menu.Items.AddRange(menuItems);

        var profiles = new List<ProfileInfo>()
        {
            new() { Id = 1 },
            new() { Id = 2 },
        };

        ProfileHandler callback = (profileInfos) => { };

        // Act
        _worker.InsertProfileMenuItems(menu.Items, profiles, callback);

        // Assert
        Assert.IsTrue(menu.Items.Count == 5);
        Assert.IsTrue(menu.Items[2].Name?.StartsWith(TrayConstants.ItemNames.ProfilePrefix) ?? false);
        Assert.IsTrue(menu.Items[3].Name?.StartsWith(TrayConstants.ItemNames.ProfilePrefix) ?? false);

        // Cleanup
        DisposeMenu(menu);
    }

    [TestMethod]
    public void InsertPlaceholderProfileMenuItem_InsertsItemAtInsertPoint()
    {
        // Arrange
        var menu = new ContextMenuStrip();
        var menuItems = new ToolStripItem[]
        {
            new ToolStripMenuItem() { Name = "Dummy Item 1" },
            new ToolStripSeparator() { Name = TrayConstants.ItemNames.ProfilesInsertPoint },
            new ToolStripMenuItem() { Name = "Dummy Item 2" },
        };
        menu.Items.AddRange(menuItems);

        // Act
        _worker.InsertPlaceholderProfileMenuItem(menu.Items);

        // Assert
        int placeholderProfileCount = menu.Items
            .Find(TrayConstants.ItemNames.PlaceholderProfile, true)
            .OfType<ToolStripMenuItem>()
            .Count();
        Assert.IsTrue(placeholderProfileCount == 1);
        Assert.IsTrue(menu.Items.Count == 4);
        Assert.AreEqual(TrayConstants.ItemNames.PlaceholderProfile, menu.Items[2].Name);

        // Cleanup
        menu.Dispose();
    }

    [TestMethod]
    public void PlaceholderProfileMenuItemExists_ReturnsFalseIfNotExists()
    {
        // Arrange
        var menu = new ContextMenuStrip();

        // Act
        var isItemFound = _worker.PlaceholderProfileMenuItemExists(menu.Items);

        // Assert
        Assert.IsFalse(isItemFound);

        // Cleanup
        menu.Dispose();
    }

    [TestMethod]
    public void PlaceholderProfileMenuItemExists_ReturnsTrueIfExists()
    {
        // Arrange
        var menu = new ContextMenuStrip();
        var placeholderProfile = TrayMenuItems.PlaceholderProfile;
        menu.Items.Add(placeholderProfile);

        // Act
        var isItemFound = _worker.PlaceholderProfileMenuItemExists(menu.Items);

        // Assert
        Assert.IsTrue(isItemFound);

        // Cleanup
        menu.Dispose();
    }

    [TestMethod]
    public void ClearPlaceholderProfileMenuItems_RemovesItems()
    {
        // Arrange
        var menu = new ContextMenuStrip();
        var placeholderProfile = TrayMenuItems.PlaceholderProfile;
        menu.Items.Add(placeholderProfile);

        // Act
        _worker.ClearPlaceholderProfileMenuItems(menu.Items);

        // Assert
        Assert.IsFalse(menu.Items.Contains(placeholderProfile));

        // Cleanup
        menu.Dispose();
    }

    #endregion

    private void DisposeMenu(ContextMenuStrip menu)
    {
        menu.Items?.OfType<ToolStripItem>().ToList().ForEach(item => item.Dispose());
        menu.Dispose();
    }
}
