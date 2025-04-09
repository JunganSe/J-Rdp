using Core.Profiles;

namespace WinApp.Tray;

internal static class TrayMenuItems
{
    public static ToolStripMenuItem ToggleConsole(Action<bool> callback)
    {
        var menuItem = new ToolStripMenuItem()
        {
            Name = TrayConstants.ItemNames.ToggleConsole,
            Text = TrayConstants.ItemTexts.ToggleConsole,
            CheckOnClick = true,
        };
        menuItem.Click += TrayMenuEvents.OnClick_ToggleConsole(callback);
        return menuItem;
    }

    public static ToolStripMenuItem ToggleLogToFile = new(null, null, TrayMenuEvents.OnClick_ToggleLogToFile)
    {
        Name = TrayConstants.ItemNames.ToggleLogToFile,
        Text = TrayConstants.ItemTexts.ToggleLogToFile,
        CheckOnClick = true,
    };

    public static ToolStripMenuItem OpenConfigFile(Action callback)
    {
        var menuItem = new ToolStripMenuItem()
        {
            Name = TrayConstants.ItemNames.OpenConfigFile,
            Text = TrayConstants.ItemTexts.OpenConfigFile,
        };
        menuItem.Click += TrayMenuEvents.OnClick_OpenConfigFile(callback);
        return menuItem;
    }

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

    public static ToolStripMenuItem Profile(ProfileInfo profileInfo, ProfileHandler callback)
    {
        var menuItem = new ToolStripMenuItem()
        {
            Name = $"{TrayConstants.ItemNames.ProfilePrefix}{profileInfo.Id}",
            Text = profileInfo.Name,
            Tag = profileInfo,
            Checked = profileInfo.Enabled,
        };
        menuItem.Click += TrayMenuEvents.OnClick_Profile(callback);
        return menuItem;
    }
}
