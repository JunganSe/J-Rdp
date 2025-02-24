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

    public void UpdateMenuProfiles(List<ProfileInfo> profileInfos)
    {
        if (NotifyIcon?.ContextMenuStrip?.Items == null)
            return;

        RemoveAllProfileMenuItems(NotifyIcon.ContextMenuStrip.Items);

        if (profileInfos.Count > 0)
            InsertProfileMenuItems(NotifyIcon.ContextMenuStrip.Items, profileInfos);
        else
            InsertPlaceholderProfileMenuItem(NotifyIcon.ContextMenuStrip.Items);
    }

    private void RemoveAllProfileMenuItems(ToolStripItemCollection menuItems)
    {
        menuItems.OfType<ToolStripMenuItem>()
            .Where(menuItem => menuItem.Name?.StartsWith(TrayConstants.ItemNames.ProfilePrefix) ?? false)
            .ToList()
            .ForEach(menuItem =>
            {
                menuItems.Remove(menuItem);
                menuItem.Dispose();
            });
    }

    private void InsertProfileMenuItems(ToolStripItemCollection menuItems, List<ProfileInfo> profileInfos)
    {
        int insertIndex = GetProfilesInsertIndex(menuItems);

        foreach (var profileInfo in profileInfos)
        {
            var menuItem = TrayMenuItems.Profile(profileInfo.Id, profileInfo.Name, profileInfo.Enabled);
            menuItems.Insert(insertIndex++, menuItem);
        }
    }

    private void InsertPlaceholderProfileMenuItem(ToolStripItemCollection menuItems)
    {
        int insertIndex = GetProfilesInsertIndex(menuItems);

        var menuItem = new ToolStripMenuItem()
        {
            Text = "No profiles found",
            Enabled = false,
        };

        menuItems.Insert(insertIndex, menuItem);
    }

    private int GetProfilesInsertIndex(ToolStripItemCollection menuItems) =>
        Math.Max(0, menuItems.IndexOfKey(TrayConstants.ItemNames.ProfilesInsertPoint));

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
        if (NotifyIcon?.ContextMenuStrip?.Items != null)
        {
            foreach (ToolStripItem item in NotifyIcon.ContextMenuStrip.Items)
                item.Dispose();
        }
        NotifyIcon?.ContextMenuStrip?.Dispose();
        NotifyIcon?.Dispose();
    }
}
