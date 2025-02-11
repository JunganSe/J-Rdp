namespace WinApp.Tray;

internal class TrayManager
{
    public NotifyIcon? NotifyIcon { get; private set; }

    internal void InitializeNotifyIconWithContextMenu()
    {
        NotifyIcon = new NotifyIcon()
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
        contextMenu.Items.Add(GetMenuItemFromProfile(2, "yass", true));

        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(TrayMenuItems.Exit);
        contextMenu.Items.Add(TrayMenuItems.Close);

        return contextMenu;
    }

    public void SetMenuState_ShowConsole(bool isChecked) =>
        SetMenuCheckedState(TrayConstants.ItemNames.ToggleConsole, isChecked);

    public void SetMenuState_LogToFile(bool isChecked) =>
        SetMenuCheckedState(TrayConstants.ItemNames.ToggleLogToFile, isChecked);

    private void SetMenuCheckedState(string itemName, bool isChecked)
    {
        if (NotifyIcon?.ContextMenuStrip?.Items == null)
            return;

        var menuItem = NotifyIcon.ContextMenuStrip.Items
            .Find(itemName, true)
            .OfType<ToolStripMenuItem>()
            .FirstOrDefault();
        if (menuItem != null)
            menuItem.Checked = isChecked;
    }

    public void DisposeMenu()
    {
        NotifyIcon?.ContextMenuStrip?.Dispose();
        NotifyIcon?.Dispose();
    }

    public ToolStripMenuItem GetMenuItemFromProfile(int index, string name, bool isActive) =>
        new(null, null, TrayMenuEvents.OnClick_Profile)
        {
            Name = $"Profile{index}",
            Text = name,
            Tag = index,
            CheckOnClick = true,
            Checked = isActive,
        };
}
