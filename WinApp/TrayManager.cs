namespace WinApp;

internal class TrayManager
{
    internal static NotifyIcon GetNotifyIcon()
    {
        return new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Visible = true,
        };
    }
}
