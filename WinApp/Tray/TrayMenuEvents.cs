using Auxiliary;
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

    public static void OnClick_Profile(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem
            && menuItem.Tag is ProfileInfo profileInfo)
        {
            bool isCtrlHeld = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            // TODO: Call something in Core to enable/disable profile based on profileIndex and isCtrlHeld.
            // 1. Get profileInfos (tags) from all profile menu items.
            // 2. Toggle enabled state of this profileInfo.
            // 3. Set enabled state of other profileInfos based on isCtrlHeld.
            // 4. Call CoreManager.UpdateProfilesEnabledState(profileInfos). But how to get CoreManager instance?
        }
    }
}
