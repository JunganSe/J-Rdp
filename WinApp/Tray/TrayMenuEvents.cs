using Auxiliary;
using Core.Delegates;
using Core.Models;
using WinApp.Managers;

namespace WinApp.Tray;

internal static class TrayMenuEvents
{
    public static EventHandler OnClick_ToggleConsole(Action<bool> callback_ToggleConsole)
    {
        return (object? sender, EventArgs e) =>
        {
            if (sender is not ToolStripMenuItem menuItem)
                return;

            callback_ToggleConsole.Invoke(menuItem.Checked);
        };
    }

    public static void OnClick_ToggleLogToFile(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem)
            LogManager.SetFileLogging(menuItem.Checked);
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
            if (!isCtrlHeld && !profileInfo.Enabled)
                profileInfos.ForEach(pi => pi.Enabled = false);

            profileInfo.Enabled = menuItem.Checked;

            callback.Invoke(profileInfos);
        };
    }

    private static List<ProfileInfo> GetProfileInfosFromMenuItems(ToolStripItemCollection items) =>
        items.OfType<ToolStripMenuItem>()
             .Where(item => item.Tag is ProfileInfo)
             .Select(item => (ProfileInfo)item.Tag!)
             .ToList();
}
