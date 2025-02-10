namespace WinApp.Tray;

internal class TrayManager
{
    internal NotifyIcon GetNotifyIcon()
    {
        return new NotifyIcon
        {
            Text = "J-Rdp",
            Icon = SystemIcons.Application,
            Visible = true,
            ContextMenuStrip = GetContextMenu(),
        };
    }

    private ContextMenuStrip GetContextMenu()
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

    internal void SetMenuState_ShowConsole(ContextMenuStrip menu, bool isChecked)
    {
        SetMenuCheckedState(menu, TrayConstants.ItemNames.ToggleConsole, isChecked);
    }

    internal void SetMenuState_LogToFile(ContextMenuStrip menu, bool isChecked)
    {
        SetMenuCheckedState(menu, TrayConstants.ItemNames.ToggleLogToFile, isChecked);
    }

    private void SetMenuCheckedState(ContextMenuStrip menu, string itemName, bool isChecked)
    {
        menu.Items.Find(itemName, true).OfType<ToolStripMenuItem>().First().Checked = isChecked;
    }
}
