using Core.Configs;
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

    public void UpdateMenuState(ConfigInfo configInfo)
    {
        if (_notifyIcon?.ContextMenuStrip is null)
        {
            _logger.Error("Can not update tray context menu state. Context menu is missing.");
            return;
        }

        if (configInfo.ShowLogConsole is not null)
            SetMenuState_ShowConsole(configInfo.ShowLogConsole.Value);

        if (configInfo.LogToFile is not null)
            SetMenuState_LogToFile(configInfo.LogToFile.Value);

        if (configInfo.Profiles is not null)
            UpdateMenuProfiles(configInfo.Profiles);
    }

    public void SetMenuState_ShowConsole(bool showConsole)
    {
        if (_notifyIcon?.ContextMenuStrip is null)
            return;

        _trayWorker.SetMenuCheckedState(_notifyIcon.ContextMenuStrip, TrayConstants.ItemNames.ToggleConsole, showConsole);
    }

    public void SetMenuState_LogToFile(bool logToFile)
    {
        if (_notifyIcon?.ContextMenuStrip is null)
            return;

        _trayWorker.SetMenuCheckedState(_notifyIcon.ContextMenuStrip, TrayConstants.ItemNames.ToggleFileLogging, logToFile);
    }

    /// <summary>
    /// Updates the profile menu items in the tray context menu to reflect the provided profileInfos.
    /// </summary>
    /// <remarks> If no profiles are provided, a disabled placeholder profile will be used. </remarks>
    public void UpdateMenuProfiles(List<ProfileInfo> profileInfos)
    {
        var menuItems = _notifyIcon?.ContextMenuStrip?.Items;
        if (menuItems is null)
            return;

        _trayWorker.RemoveAllProfileMenuItems(menuItems);

        if (profileInfos.Count > 0)
        {
            _trayWorker.ClearPlaceholderProfileMenuItems(menuItems);
            _trayWorker.InsertProfileMenuItems(menuItems, profileInfos, _callbacks.ProfilesActiveStateChanged);
        }
        else if (!_trayWorker.PlaceholderProfileMenuItemExists(menuItems))
            _trayWorker.InsertPlaceholderProfileMenuItem(menuItems);
    }

    public void DisposeTray() =>
        _trayWorker.DisposeTray(_notifyIcon);
}
