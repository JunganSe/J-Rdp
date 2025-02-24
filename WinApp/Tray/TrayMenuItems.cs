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

    public static ToolStripMenuItem Close = new(null, null, TrayMenuEvents.OnClick_Close)
    {
        Name = TrayConstants.ItemNames.Close,
        Text = TrayConstants.ItemTexts.Close,
    };

    public static ToolStripMenuItem Profile(int index, string name, bool isEnabled) =>
        new(null, null, TrayMenuEvents.OnClick_Profile)
        {
            Name = $"{TrayConstants.ItemNames.ProfilePrefix}{index}",
            Text = name,
            Tag = index,
            Checked = isEnabled,
        };
}
