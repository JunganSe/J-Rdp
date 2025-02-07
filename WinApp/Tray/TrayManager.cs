namespace WinApp.Tray;

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
        var contextMenu = new ContextMenuStrip()
        {
            AutoClose = false,
        };
        
        contextMenu.Items.Add(TrayMenuItems.ShowConsole);
        contextMenu.Items.Add(TrayMenuItems.LogToFile);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(TrayMenuItems.Exit);
        contextMenu.Items.Add(TrayMenuItems.Cancel);

        return contextMenu;
    }
}
