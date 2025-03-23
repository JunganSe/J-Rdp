using Core.Delegates;
using Core.Models;
using NLog;

namespace WinApp.Tray;

internal class TrayWorker
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    #region Creation

    public NotifyIcon? CreateNotifyIcon() => new NotifyIcon()
    {
        Text = "J-Rdp",
        Icon = SystemIcons.Application,
        Visible = true,
    };

    public ContextMenuStrip? CreateContextMenu(Action<bool> _callback_ToggleConsole)
    {
        var contextMenu = new ContextMenuStrip() { AutoClose = false, };

        contextMenu.Items.Add(TrayMenuItems.ToggleConsole(_callback_ToggleConsole));
        contextMenu.Items.Add(TrayMenuItems.ToggleLogToFile);

        contextMenu.Items.Add(new ToolStripSeparator() { Name = TrayConstants.ItemNames.ProfilesInsertPoint });
        contextMenu.Items.Add(new ToolStripSeparator());

        contextMenu.Items.Add(TrayMenuItems.Exit);
        contextMenu.Items.Add(TrayMenuItems.Close);

        return contextMenu;
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
            var menuItem = new ToolStripMenuItem()
            {
                Text = "No profiles found",
                Enabled = false,
            };
            menuItems.Insert(insertIndex, menuItem);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to insert placeholder profile menu item.");
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
