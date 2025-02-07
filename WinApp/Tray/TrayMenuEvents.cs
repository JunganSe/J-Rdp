namespace WinApp.Tray;

internal static class TrayMenuEvents
{
    public static void OnClick_ShowConsole(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem)
        {
            // TODO: Toggle console visibility.
        }
    }

    public static void OnClick_LogToFile(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem)
        {
            // TODO: Toggle logging to file.
        }
    }

    public static void OnClick_Exit(object? sender, EventArgs e)
    {
        Application.Exit();
    }

    public static void OnClick_Cancel(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem 
            && menuItem.Owner is ContextMenuStrip contextMenu)
        {
            contextMenu.Close();
        }
    }
}
