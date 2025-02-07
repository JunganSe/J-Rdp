namespace WinApp.Tray;

internal static class TrayMenuItems
{
    public static ToolStripMenuItem ToggleConsole = new(null, null, TrayMenuEvents.OnClick_ToggleConsole)
    {
        Name = TrayConstants.ItemNames.ToggleConsole,
        Text = TrayConstants.ItemTexts.ToggleConsole,
        CheckOnClick = true,
    };

    public static ToolStripMenuItem ToggleLogToFile = new(null, null, TrayMenuEvents.OnClick_ToggleLogToFile)
    {
        Name = TrayConstants.ItemNames.ToggleLogToFile,
        Text = TrayConstants.ItemTexts.ToggleLogToFile,
        CheckOnClick = true,
    };

    public static ToolStripMenuItem Exit = new(null, null, TrayMenuEvents.OnClick_Exit)
    {
        Name = TrayConstants.ItemNames.Exit,
        Text = TrayConstants.ItemTexts.Exit,
    };

    public static ToolStripMenuItem Cancel = new(null, null, TrayMenuEvents.OnClick_Cancel)
    {
        Name = TrayConstants.ItemNames.Cancel,
        Text = TrayConstants.ItemTexts.Cancel,
    };
}
