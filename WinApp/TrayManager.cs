namespace WinApp;

internal static class TrayManager
{
    internal static NotifyIcon GetNotifyIcon()
    {
        return new NotifyIcon
        {
            Text = "J-Rdp",
            Icon = SystemIcons.Application,
            Visible = true,
            ContextMenuStrip = GetContextMenu(),
        };
    }

    private static ContextMenuStrip GetContextMenu()
    {
        var contextMenu = new ContextMenuStrip();

        var item1 = new ToolStripMenuItem("Item 1", null, OnClick_Item1) { Tag = "Tag1" };
        contextMenu.Items.Add(item1);

        var exitMenuItem = new ToolStripMenuItem("Exit", null, OnClick_Exit);
        contextMenu.Items.Add(exitMenuItem);

        return contextMenu;
    }

    private static void OnClick_Item1(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem)
        {
            var tagValue = menuItem.Tag?.ToString();
            MessageBox.Show($"Clicked Item 1 with Tag: {tagValue}");
        }
    }

    private static void OnClick_Exit(object? sender, EventArgs e)
    {
        Application.Exit();
    }
}
