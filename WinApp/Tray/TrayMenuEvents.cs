using Core.Profiles;

namespace WinApp.Tray;

internal static class TrayMenuEvents
{
    public static EventHandler OnClick_ToggleConsole(Action<bool> callback)
    {
        return (object? sender, EventArgs e) =>
        {
            if (sender is not ToolStripMenuItem menuItem)
                return;

            bool showConsole = menuItem.Checked;
            callback.Invoke(showConsole);
        };
    }

    public static EventHandler OnClick_ToggleLogToFile(Action<bool> callback)
    {
        return (object? sender, EventArgs e) =>
        {
            if (sender is not ToolStripMenuItem menuItem)
                return;

            bool logToFile = menuItem.Checked;
            callback.Invoke(logToFile);
        };
    }

    public static EventHandler OnClick_OpenLogsFolder(Action callback)
    {
        return (object? sender, EventArgs e) =>
        {
            if (sender is not ToolStripMenuItem menuItem)
                return;

            callback.Invoke();
        };
    }

    public static EventHandler OnClick_OpenConfigFile(Action callback)
    {
        return (object? sender, EventArgs e) =>
        {
            if (sender is not ToolStripMenuItem menuItem)
                return;

            callback.Invoke();
        };
    }

    public static void OnClick_Exit(object? sender, EventArgs e)
    {
        Application.Exit();
    }

    public static void OnClick_Close(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem
            && menuItem.Owner is ContextMenuStrip contextMenu)
        {
            contextMenu.Close();
        }
    }

    public static EventHandler OnClick_Profile(ProfileHandler callback)
    {
        return (object? sender, EventArgs e) =>
        {
            if (sender is not ToolStripMenuItem menuItem || menuItem.Tag is not ProfileInfo profileInfo)
                return;

            if (menuItem.Owner is null)
                throw new InvalidOperationException($"Menu item '{menuItem.Text}' has no owner.");

            var profileInfos = GetProfileInfosFromMenuItems(menuItem.Owner.Items);

            bool isCtrlHeld = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            if (isCtrlHeld)
            {
                // Toggle the clicked profile.
                menuItem.Checked = !menuItem.Checked;
                profileInfo.Enabled = menuItem.Checked;
            }
            else
            {
                // Enable the clicked profile and disable the others.
                profileInfos.ForEach(pi => pi.Enabled = false);
                profileInfo.Enabled = true;
                menuItem.Checked = true;
            }

            callback.Invoke(profileInfos);
        };
    }

    private static List<ProfileInfo> GetProfileInfosFromMenuItems(ToolStripItemCollection items) =>
        items.OfType<ToolStripMenuItem>()
             .Where(item => item.Tag is ProfileInfo)
             .Select(item => (ProfileInfo)item.Tag!)
             .ToList();
}
