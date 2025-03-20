using Core.Delegates;
using Core.Models;
using NLog;
using WinApp.Tray;

namespace WinApp.Managers;

internal class TrayManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
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
            ContextMenuStrip = CreateContextMenu(),
        };
    }

    private ContextMenuStrip? CreateContextMenu()
    {
        if (_callback_ToggleConsole is null)
        {
            _logger.Error("Can not create context menu. Callback 'ToggleConsole' is missing.");
            return null;
        }

        var contextMenu = new ContextMenuStrip() { AutoClose = false, };

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
        var menuItems = _notifyIcon?.ContextMenuStrip?.Items;
        if (menuItems is null)
        {
            _logger.Error("Can not update profiles in context menu. Context menu is missing.");
            return;
        }

        RemoveAllProfileMenuItems(menuItems);

        if (profileInfos.Count > 0)
            InsertProfileMenuItems(menuItems, profileInfos);
        else
            InsertPlaceholderProfileMenuItem(menuItems);
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
        {
            _logger.Error("Can not insert profile menu items into context menu. Callback is missing.");
            return;
        }

        int insertIndex = GetProfilesInsertIndex(menuItems);
        foreach (var profileInfo in profileInfos)
        {
            var menuItem = TrayMenuItems.Profile(profileInfo, _callback_ProfilesActiveStateChanged);
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

    public void SetMenuState_ShowConsole(bool showConsole) =>
        SetMenuCheckedState(TrayConstants.ItemNames.ToggleConsole, showConsole);

    public void SetMenuState_LogToFile(bool logToFile) =>
        SetMenuCheckedState(TrayConstants.ItemNames.ToggleLogToFile, logToFile);

    private void SetMenuCheckedState(string itemName, bool isChecked)
    {
        if (_notifyIcon?.ContextMenuStrip?.Items is null)
            return;

        if (_notifyIcon.ContextMenuStrip.InvokeRequired)
        {
            // Call this method from the UI thread instead.
            _notifyIcon.ContextMenuStrip.Invoke(() => SetMenuCheckedState(itemName, isChecked));
            return;
        }

        var menuItem = _notifyIcon.ContextMenuStrip.Items
            .Find(itemName, true)
            .OfType<ToolStripMenuItem>()
            .FirstOrDefault();
        if (menuItem is not null)
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
