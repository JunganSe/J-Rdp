namespace WinApp;

internal class TrayManager
{
    internal static NotifyIcon GetNotifyIcon()
    {
        return new NotifyIcon
        {
            Text = "J-Rdp",
            Icon = SystemIcons.Application,
            Visible = true,
        };
    }
}
