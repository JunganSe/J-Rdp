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
        
        contextMenu.Items.Add(new ToolStripMenuItem(null, null, OnClick_ShowConsole)
        {
            Name = TrayConstants.ItemNames.ShowConsole,
            Text = TrayConstants.ItemTexts.ShowConsole,
            CheckOnClick = true,
        });
        
        contextMenu.Items.Add(new ToolStripMenuItem(null, null, OnClick_LogToFile)
        {
            Name = TrayConstants.ItemNames.LogToFile,
            Text = TrayConstants.ItemTexts.LogToFile,
            CheckOnClick = true,
        });

        contextMenu.Items.Add(new ToolStripSeparator());

        contextMenu.Items.Add(new ToolStripMenuItem(null, null, OnClick_Exit)
        {
            Name = TrayConstants.ItemNames.Exit,
            Text = TrayConstants.ItemTexts.Exit,
        });

        contextMenu.Items.Add(new ToolStripMenuItem(null, null, (s, e) => contextMenu.Close())
        {
            Name = TrayConstants.ItemNames.Cancel,
            Text = TrayConstants.ItemTexts.Cancel,
        });

        return contextMenu;
    }
}
