#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable IDE0039 // Use local function

using Core.Profiles;
using System.Windows.Forms;
using WinApp.Tray;

namespace WinAppTests.Tray;

[TestClass]
[DoNotParallelize] // Avoids issues where some tests randomly fail. It seems that ContextMenuStrip is sharing resources across tests when run in parallel.
public class TrayWorkerTests_MenuItems
{
    private TrayWorker _worker;

    [TestInitialize]
    public void InitializeTest()
    {
        _worker = new TrayWorker();
    }

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
        menu.Dispose();
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
        Assert.AreEqual(5, menu.Items.Count);
        Assert.IsTrue(menu.Items[2].Name?.StartsWith(TrayConstants.ItemNames.ProfilePrefix) ?? false);
        Assert.IsTrue(menu.Items[3].Name?.StartsWith(TrayConstants.ItemNames.ProfilePrefix) ?? false);

        // Cleanup
        menu.Dispose();
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
        Assert.AreEqual(1, placeholderProfileCount);
        Assert.AreEqual(4, menu.Items.Count);
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
        bool isItemFound = _worker.PlaceholderProfileMenuItemExists(menu.Items);

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
        bool isItemFound = _worker.PlaceholderProfileMenuItemExists(menu.Items);

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
}
