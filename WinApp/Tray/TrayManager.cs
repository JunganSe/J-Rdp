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
        
        contextMenu.Items.Add(TrayMenuItems.ToggleConsole);
        contextMenu.Items.Add(TrayMenuItems.ToggleLogToFile);

        contextMenu.Items.Add(new ToolStripSeparator());
        // TODO: Add profiles here, or an unclickable item if no profiles are available. Issue #61
        
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(TrayMenuItems.Exit);
        contextMenu.Items.Add(TrayMenuItems.Close);

        return contextMenu;
    }
}
