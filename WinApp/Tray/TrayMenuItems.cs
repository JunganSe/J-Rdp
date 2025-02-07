namespace WinApp.Tray;

internal static class TrayMenuItems
{
    public static ToolStripMenuItem ShowConsole = new ToolStripMenuItem(null, null, OnClick_ShowConsole)
    {
        Name = TrayConstants.ItemNames.ShowConsole,
        Text = TrayConstants.ItemTexts.ShowConsole,
        CheckOnClick = true,
    };

    public static ToolStripMenuItem LogToFile = new ToolStripMenuItem(null, null, OnClick_LogToFile)
    {
        Name = TrayConstants.ItemNames.LogToFile,
        Text = TrayConstants.ItemTexts.LogToFile,
        CheckOnClick = true,
    };

    public static ToolStripMenuItem Exit = new ToolStripMenuItem(null, null, OnClick_Exit)
    {
        Name = TrayConstants.ItemNames.Exit,
        Text = TrayConstants.ItemTexts.Exit,
    };

    public static ToolStripMenuItem Cancel = new ToolStripMenuItem(null, null, OnClick_Close)
    {
        Name = TrayConstants.ItemNames.Cancel,
        Text = TrayConstants.ItemTexts.Cancel,
    };
}
