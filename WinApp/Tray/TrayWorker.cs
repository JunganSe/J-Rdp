using Core.Profiles;
using NLog;

namespace WinApp.Tray;

internal class TrayWorker
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    #region Creation

    public NotifyIcon? CreateNotifyIcon()
    {
        try
        {
            using var stream = new MemoryStream(Properties.Resources.J_Rdp_icon);
            var icon = new Icon(stream);

            return new NotifyIcon()
            {
                Text = TrayConstants.General.Title,
                Icon = icon,
                Visible = true,
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create tray icon.");
            return null;
        }
    }

    public ContextMenuStrip? CreateContextMenu(
        Action<bool>? callback_ToggleConsole,
        Action? callback_OpenConfigFile)
    {
        if (callback_ToggleConsole is null)
        {
            _logger.Error("Can not create context menu. Callback 'ToggleConsole' is missing.");
            return null;
        }
        if (callback_OpenConfigFile is null)
        {
            _logger.Error("Can not create context menu. Callback 'OpenConfigFile' is missing.");
            return null;
        }

        var contextMenu = new ContextMenuStrip() { AutoClose = false, };

        contextMenu.Items.Add(TrayMenuItems.ToggleConsole(callback_ToggleConsole));
        contextMenu.Items.Add(TrayMenuItems.ToggleLogToFile);
        contextMenu.Items.Add(TrayMenuItems.OpenConfigFile(callback_OpenConfigFile));

        contextMenu.Items.Add(new ToolStripSeparator() { Name = TrayConstants.ItemNames.ProfilesInsertPoint });
        contextMenu.Items.Add(new ToolStripSeparator());

        contextMenu.Items.Add(TrayMenuItems.Exit);
        contextMenu.Items.Add(TrayMenuItems.Close);

        return contextMenu;
    }

    #endregion

    #region Menu checked state

    public void SetMenuCheckedState(ContextMenuStrip menu, string itemName, bool isChecked)
    {
        try
        {
            if (menu.InvokeRequired)
                menu.Invoke(() => SetMenuItemsCheckedState(menu.Items, itemName, isChecked)); // Call from the UI thread.
            else
                SetMenuItemsCheckedState(menu.Items, itemName, isChecked); // Call normally.
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to set menu item '{itemName}' checked state.");
        }
    }

    private void SetMenuItemsCheckedState(ToolStripItemCollection menuItems, string itemName, bool isChecked)
    {
        menuItems
            .Find(itemName, true)
            .OfType<ToolStripMenuItem>()
            .ToList()
            .ForEach(menuItem => menuItem.Checked = isChecked);
    }

    #endregion

    #region Profile menu items

    public void RemoveAllProfileMenuItems(ToolStripItemCollection menuItems)
    {
        try
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
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to remove profile menu items.");
        }
    }

    public void InsertProfileMenuItems(
        ToolStripItemCollection menuItems,
        List<ProfileInfo> profileInfos,
        ProfileHandler callback_ProfilesActiveStateChanged)
    {
        try
        {
            int insertIndex = GetProfilesInsertIndex(menuItems);
            foreach (var profileInfo in profileInfos)
            {
                var menuItem = TrayMenuItems.Profile(profileInfo, callback_ProfilesActiveStateChanged);
                menuItems.Insert(insertIndex++, menuItem);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to insert profile menu item.");
        }
    }

    public void InsertPlaceholderProfileMenuItem(ToolStripItemCollection menuItems)
    {
        try
        {
            int insertIndex = GetProfilesInsertIndex(menuItems);
            var menuItem = TrayMenuItems.PlaceholderProfile;
            menuItems.Insert(insertIndex, menuItem);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to insert placeholder profile menu item.");
        }
    }

    public void ClearPlaceholderProfileMenuItems(ToolStripItemCollection menuItems)
    {
        try
        {
            menuItems
                .Find(TrayConstants.ItemNames.PlaceholderProfile, true)
                .OfType<ToolStripMenuItem>()
                .ToList()
                .ForEach(menuItem =>
                {
                    menuItems.Remove(menuItem);
                    menuItem.Dispose();
                });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to clear placeholder profile menu items.");
        }
    }

    private int GetProfilesInsertIndex(ToolStripItemCollection menuItems) =>
        1 + menuItems.IndexOfKey(TrayConstants.ItemNames.ProfilesInsertPoint);

    #endregion

    #region Cleanup

    public void DisposeTray(NotifyIcon? notifyIcon)
    {
        try
        {
            notifyIcon?.ContextMenuStrip?.Items?
                .OfType<ToolStripItem>()
                .ToList()
                .ForEach(item => item.Dispose());
            notifyIcon?.ContextMenuStrip?.Dispose();
            notifyIcon?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to dispose tray icon and/or context menu.");
        }
    }

    #endregion
}
