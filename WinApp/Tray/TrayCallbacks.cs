using Core.Profiles;

namespace WinApp.Tray;

internal class TrayCallbacks
{
    public Action<bool> ToggleConsole { get; set; } = (_)=> { };
    public Action OpenLogsFolder { get; set; } = () => { };
    public Action OpenConfigFile { get; set; } = () => { };
    public ProfileHandler ProfilesActiveStateChanged { get; set; } = (_) => { };
}
