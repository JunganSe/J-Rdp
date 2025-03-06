using Auxiliary;
using Core.Delegates;
using Core.Models;
using WinApp.Managers;

namespace WinApp.Tray;

internal static class TrayMenuEvents
{
    public static void OnClick_ToggleConsole(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem)
            ConsoleManager.SetVisibility(menuItem.Checked);
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
            if (sender is ToolStripMenuItem menuItem
                && menuItem.Tag is ProfileInfo profileInfo)
            {
                if (menuItem.Owner is null)
                    throw new InvalidOperationException($"Menu item '{menuItem.Text}' has no owner.");

                var profileInfos = menuItem.Owner.Items
                    .OfType<ToolStripMenuItem>()
                    .Where(item => item.Tag is ProfileInfo and not null)
                    .Select(item => (ProfileInfo)item.Tag!)
                    .ToList();

                bool isCtrlHeld = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                if (!isCtrlHeld && !profileInfo.Enabled)
                    profileInfos.ForEach(pi => pi.Enabled = false);

                profileInfo.Enabled = menuItem.Checked;

                callback.Invoke(profileInfos);
            }
        };
    }
}
