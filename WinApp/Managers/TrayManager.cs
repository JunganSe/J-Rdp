using Core.Delegates;
using Core.Models;
using WinApp.Tray;

namespace WinApp.Managers;

internal class TrayManager
{
    private NotifyIcon? _notifyIcon;
    private Action<bool>? _callback_ToggleConsole;
    private ProfileHandler? _callback_ProfilesActiveStateChanged;

    public void SetCallback_ToggleConsole(Action<bool> callback) =>
        _callback_ToggleConsole = callback;
    
    public void SetCallback_ProfilesActiveStateChanged(ProfileHandler callback) =>
        _callback_ProfilesActiveStateChanged = callback;

    public void InitializeNotifyIconWithContextMenu()
    {
        _notifyIcon = new NotifyIcon()
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

        if (_callback_ToggleConsole is null)
            throw new InvalidOperationException("Can not create menu item for toggling console. Callback is missing.");
        contextMenu.Items.Add(TrayMenuItems.ToggleConsole(_callback_ToggleConsole));

        contextMenu.Items.Add(TrayMenuItems.ToggleLogToFile);
        contextMenu.Items.Add(new ToolStripSeparator() { Name = TrayConstants.ItemNames.ProfilesInsertPoint });

        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(TrayMenuItems.Exit);
        contextMenu.Items.Add(TrayMenuItems.Close);

        return contextMenu;
    }

    public void UpdateMenuProfiles(List<ProfileInfo> profileInfos)
    {
        if (_notifyIcon?.ContextMenuStrip?.Items == null)
            return;

        RemoveAllProfileMenuItems(_notifyIcon.ContextMenuStrip.Items);

        if (profileInfos.Count > 0)
            InsertProfileMenuItems(_notifyIcon.ContextMenuStrip.Items, profileInfos);
        else
            InsertPlaceholderProfileMenuItem(_notifyIcon.ContextMenuStrip.Items);
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
        if (_callback_ProfilesActiveStateChanged is null)
            throw new InvalidOperationException("Can not insert profile menu items. Callback is missing.");

        int insertIndex = GetProfilesInsertIndex(menuItems);

        foreach (var profileInfo in profileInfos)
        {
            var menuItem = TrayMenuItems.Profile(profileInfo);
            menuItem.Click += TrayMenuEvents.OnClick_Profile(_callback_ProfilesActiveStateChanged);
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
        1 + menuItems.IndexOfKey(TrayConstants.ItemNames.ProfilesInsertPoint);

    public void SetMenuState_ShowConsole(bool isChecked) =>
        SetMenuCheckedState(TrayConstants.ItemNames.ToggleConsole, isChecked);

    public void SetMenuState_LogToFile(bool isChecked) =>
        SetMenuCheckedState(TrayConstants.ItemNames.ToggleLogToFile, isChecked);

    private void SetMenuCheckedState(string itemName, bool isChecked)
    {
        if (_notifyIcon?.ContextMenuStrip?.Items == null)
            return;

        var menuItem = _notifyIcon.ContextMenuStrip.Items
            .Find(itemName, true)
            .OfType<ToolStripMenuItem>()
            .FirstOrDefault();
        if (menuItem != null)
            menuItem.Checked = isChecked;
    }

    public void DisposeTray()
    {
        _notifyIcon?.ContextMenuStrip?.Items?
            .OfType<ToolStripItem>()
            .ToList()
            .ForEach(item => item.Dispose());
        _notifyIcon?.ContextMenuStrip?.Dispose();
        _notifyIcon?.Dispose();
    }
}
