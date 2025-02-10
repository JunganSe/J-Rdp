using Auxiliary;

namespace WinApp.Tray;

internal static class TrayMenuEvents
{
    public static void OnClick_ToggleConsole(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem)
        {
            ConsoleManager.SetVisibility(menuItem.Checked);
        }
    }

    public static void OnClick_ToggleLogToFile(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem)
        {
            LogManager.SetFileLogging(menuItem.Checked);
        }
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
}
