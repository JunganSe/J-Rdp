using Core.Profiles;
using NLog;

namespace WinApp.Tray;

internal class TrayManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly TrayWorker _trayWorker = new();
    private TrayCallbacks _callbacks = new();
    private NotifyIcon? _notifyIcon;

    public void SetCallbacks(TrayCallbacks callbacks) =>
        _callbacks = callbacks;

    public void InitializeNotifyIconWithContextMenu()
    {
        DisposeTray();

        _notifyIcon = _trayWorker.CreateNotifyIcon();
        if (_notifyIcon is null)
            return;

        _notifyIcon.ContextMenuStrip = _trayWorker.CreateContextMenu(_callbacks);
    }

    public void UpdateMenuProfiles(List<ProfileInfo> profileInfos)
    {
        var menuItems = _notifyIcon?.ContextMenuStrip?.Items;
        if (menuItems is null)
        {
            _logger.Error("Can not update profile items in context menu. Context menu is missing.");
            return;
        }

        if (_callbacks.ProfilesActiveStateChanged is null)
        {
            _logger.Error("Can not insert profile menu items into context menu. Callback is missing.");
            return;
        }

        _trayWorker.RemoveAllProfileMenuItems(menuItems);

        if (profileInfos.Count > 0)
        {
            _trayWorker.ClearPlaceholderProfileMenuItems(menuItems);
            _trayWorker.InsertProfileMenuItems(menuItems, profileInfos, _callbacks.ProfilesActiveStateChanged);
        }
        else if (!_trayWorker.PlaceholderProfileMenuItemExists(menuItems))
            _trayWorker.InsertPlaceholderProfileMenuItem(menuItems);
    }

    public void SetMenuState_ShowConsole(bool showConsole)
    {
        if (_notifyIcon?.ContextMenuStrip is null)
        {
            _logger.Error("Can not set menu checked 'show console' state. Context menu is missing.");
            return;
        }

        _trayWorker.SetMenuCheckedState(_notifyIcon.ContextMenuStrip, TrayConstants.ItemNames.ToggleConsole, showConsole);
    }

    public void SetMenuState_LogToFile(bool logToFile)
    {
        if (_notifyIcon?.ContextMenuStrip is null)
        {
            _logger.Error("Can not set menu checked state 'log to file'. Context menu is missing.");
            return;
        }

        _trayWorker.SetMenuCheckedState(_notifyIcon.ContextMenuStrip, TrayConstants.ItemNames.ToggleLogToFile, logToFile);
    }

    public void DisposeTray() =>
        _trayWorker.DisposeTray(_notifyIcon);
}
