using NLog;

namespace WinApp.Tray;

internal class TrayWorker
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
}
