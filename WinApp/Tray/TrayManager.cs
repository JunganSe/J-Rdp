using Core.Models;

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

        contextMenu.Items.Add(new ToolStripSeparator()
        {
            Name = TrayConstants.ItemNames.ProfilesInsertPoint,
        });

        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(TrayMenuItems.Exit);
        contextMenu.Items.Add(TrayMenuItems.Close);

        return contextMenu;
    }

    // TODO: Refactor.
    public void UpdateMenuProfiles(List<ProfileInfo> profileInfos)
    {
        if (NotifyIcon?.ContextMenuStrip?.Items == null)
            return;

        // Remove existing profile items
        var existingProfileItems = NotifyIcon.ContextMenuStrip.Items
            .OfType<ToolStripMenuItem>()
            .Where(item => item.Name?.StartsWith("Profile") ?? false)
            .ToList();
        foreach (var item in existingProfileItems)
            NotifyIcon.ContextMenuStrip.Items.Remove(item);

        // Insert new profile items at the specified position
        int insertIndex = NotifyIcon.ContextMenuStrip.Items.IndexOfKey(TrayConstants.ItemNames.ProfilesInsertPoint);
        if (insertIndex == -1)
            insertIndex = NotifyIcon.ContextMenuStrip.Items.Count;

        foreach (var profileInfo in profileInfos)
        {
            var menuItem = TrayMenuItems.Profile(profileInfo.Id, profileInfo.Name, profileInfo.Enabled);
            NotifyIcon.ContextMenuStrip.Items.Insert(insertIndex++, menuItem);
        }

        // TODO: Add an unclickable item if no profiles are available.
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

    public void DisposeTray()
    {
        NotifyIcon?.ContextMenuStrip?.Dispose();
        NotifyIcon?.Dispose();
    }
}
