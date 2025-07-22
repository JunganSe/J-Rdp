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

    public static ToolStripMenuItem ToggleFileLogging(Action<bool> callback)
    {
        var menuItem = new ToolStripMenuItem()
        {
            Name = TrayConstants.ItemNames.ToggleFileLogging,
            Text = TrayConstants.ItemTexts.ToggleFileLogging,
            CheckOnClick = true,
        };
        menuItem.Click += TrayMenuEvents.OnClick_ToggleLogToFile(callback);
        return menuItem;
    }

    public static ToolStripMenuItem OpenLogsFolder(Action callback)
    {
        var menuItem = new ToolStripMenuItem()
        {
            Name = TrayConstants.ItemNames.OpenLogsFolder,
            Text = TrayConstants.ItemTexts.OpenLogsFolder,
        };
        menuItem.Click += TrayMenuEvents.OnClick_OpenLogsFolder(callback);
        return menuItem;
    }

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

    public static ToolStripMenuItem Exit()
    {
        var menuItem = new ToolStripMenuItem()
        {
            Name = TrayConstants.ItemNames.Exit,
            Text = TrayConstants.ItemTexts.Exit,
        };
        menuItem.Click += TrayMenuEvents.OnClick_Exit();
        return menuItem;
    }

    public static ToolStripMenuItem Close()
    {
        var menuItem = new ToolStripMenuItem()
        {
            Name = TrayConstants.ItemNames.Close,
            Text = TrayConstants.ItemTexts.Close,
        };
        menuItem.Click += TrayMenuEvents.OnClick_Close();
        return menuItem;
    }

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

    public static ToolStripMenuItem PlaceholderProfile = new()
    {
        Name = TrayConstants.ItemNames.PlaceholderProfile,
        Text = TrayConstants.ItemTexts.PlaceholderProfile,
        Enabled = false,
    };
}
