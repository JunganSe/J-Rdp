using Auxiliary;
using Core.Profiles;

namespace WinApp.Tray;

internal class TrayWorker
{
    private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    #region Creation

    public NotifyIcon? CreateNotifyIcon()
    {
        try
        {
            return new NotifyIcon()
            {
                Text = GetTrayIconTooltip(),
                Icon = GetTrayIcon(),
                Visible = true,
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create tray icon.");
            return null;
        }
    }

    private string GetTrayIconTooltip()
    {
        var assembly = typeof(WinApp.Program).Assembly;
        string name = AssemblyHelper.GetAssemblyName(assembly);
        string version = AssemblyHelper.GetAssemblyVersion(assembly);
        return $"{name} {version}";
    }

    private Icon GetTrayIcon()
    {
        using var iconStream = new MemoryStream(Properties.Resources.J_Rdp_icon);
        return new Icon(iconStream);
    }

    public ContextMenuStrip? CreateContextMenu(TrayCallbacks callbacks)
    {
        if (callbacks.ToggleConsole is null)
        {
            _logger.Error("Can not create context menu. Callback 'ToggleConsole' is missing.");
            return null;
        }
        if (callbacks.OpenLogsFolder is null)
        {
            _logger.Error("Can not create context menu. Callback 'OpenLogsFolder' is missing.");
            return null;
        }
        if (callbacks.OpenConfigFile is null)
        {
            _logger.Error("Can not create context menu. Callback 'OpenConfigFile' is missing.");
            return null;
        }

        var contextMenu = new ContextMenuStrip() { AutoClose = false };
        var menuItems = CreateContextMenuItems(callbacks);
        contextMenu.Items.AddRange(menuItems);
        return contextMenu;
    }

    private ToolStripItem[] CreateContextMenuItems(TrayCallbacks callbacks) =>
    [
        TrayMenuItems.ToggleConsole(callbacks.ToggleConsole),
        TrayMenuItems.ToggleLogToFile,
        TrayMenuItems.OpenLogsFolder(callbacks.OpenLogsFolder),
        TrayMenuItems.OpenConfigFile(callbacks.OpenConfigFile),

        new ToolStripSeparator() { Name = TrayConstants.ItemNames.ProfilesInsertPoint },
        new ToolStripSeparator(),

        TrayMenuItems.Exit,
        TrayMenuItems.Close,
    ];

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
            menuItems
                .OfType<ToolStripMenuItem>()
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

    public bool PlaceholderProfileMenuItemExists(ToolStripItemCollection menuItems)
    {
        return menuItems
            .Find(TrayConstants.ItemNames.PlaceholderProfile, true)
            .OfType<ToolStripMenuItem>()
            .Any();
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
            notifyIcon?.ContextMenuStrip?.Dispose(); // ContextMenuStrip.Items are disposed automatically when ContextMenuStrip is disposed.
            notifyIcon?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to dispose tray icon and/or context menu.");
        }
    }

    #endregion
}
