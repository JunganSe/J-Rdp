using Core.Delegates;
using Core.Models;
using NLog;

namespace WinApp.Tray;

internal class TrayManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly TrayWorker _trayWorker = new();
    private NotifyIcon? _notifyIcon;
    private Action<bool>? _callback_ToggleConsole;
    private ProfileHandler? _callback_ProfilesActiveStateChanged;

    public void SetCallback_ToggleConsole(Action<bool> callback) =>
        _callback_ToggleConsole = callback;

    public void SetCallback_ProfilesActiveStateChanged(ProfileHandler callback) =>
        _callback_ProfilesActiveStateChanged = callback;

    public void InitializeNotifyIconWithContextMenu()
    {
        DisposeTray();
        _notifyIcon = _trayWorker.CreateNotifyIcon();
        if (_notifyIcon is not null)
            _notifyIcon.ContextMenuStrip = _trayWorker.CreateContextMenu(_callback_ToggleConsole);
    }

    public void UpdateMenuProfiles(List<ProfileInfo> profileInfos)
    {
        var menuItems = _notifyIcon?.ContextMenuStrip?.Items;
        if (menuItems is null)
        {
            _logger.Error("Can not update profile items in context menu. Context menu is missing.");
            return;
        }

        if (_callback_ProfilesActiveStateChanged is null)
        {
            _logger.Error("Can not insert profile menu items into context menu. Callback is missing.");
            return;
        }

        _trayWorker.RemoveAllProfileMenuItems(menuItems);

        if (profileInfos.Count > 0)
            _trayWorker.InsertProfileMenuItems(menuItems, profileInfos, _callback_ProfilesActiveStateChanged);
        else
            _trayWorker.InsertPlaceholderProfileMenuItem(menuItems);
    }

    public void SetMenuState_ShowConsole(bool showConsole) =>
        SetMenuCheckedState(TrayConstants.ItemNames.ToggleConsole, showConsole);

    public void SetMenuState_LogToFile(bool logToFile) =>
        SetMenuCheckedState(TrayConstants.ItemNames.ToggleLogToFile, logToFile);

    private void SetMenuCheckedState(string itemName, bool isChecked)
    {
        try
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
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to set menu item '{itemName}' checked state.");
        }
    }

    public void DisposeTray() =>
        _trayWorker.DisposeTray(_notifyIcon);
}
