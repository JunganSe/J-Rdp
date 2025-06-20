using Core.Profiles;

namespace WinApp.Tray;

internal class TrayManagerCallbacks
{
    public Action<bool>? ToggleConsole { get; set; }
    public Action? OpenLogsFolder { get; set; }
    public Action? OpenConfigFile { get; set; }
    public ProfileHandler? ProfilesActiveStateChanged { get; set; }
}
