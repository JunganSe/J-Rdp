#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

using System.Windows.Forms;
using WinApp.Tray;

namespace WinAppTests.Tray;

[TestClass]
public class TrayWorkerTests_MenuCheckedState
{
    private TrayWorker _worker;

    [TestInitialize]
    public void InitializeTest()
    {
        _worker = new TrayWorker();
    }

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
        menu.Dispose();
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
        menu.Dispose();
    }
}
