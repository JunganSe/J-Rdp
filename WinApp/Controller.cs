using WinApp.Managers;
using WinApp.Tray;

namespace WinApp;

internal class Controller
{
    private readonly CoreManager _coreManager = new();
    private readonly TrayManager _trayManager = new();

    public void Run()
    {
        _coreManager.Run();
    }

    public void InitializeCore()
    {
        _coreManager.Initialize();
        _coreManager.SetCallback_ConfigUpdated(_trayManager.UpdateMenuProfiles);
    }

    public void InitializeTray(Arguments arguments)
    {
        _trayManager.ProfilesActiveStateChangedCallback = _coreManager.UpdateProfilesEnabledState;
        _trayManager.InitializeNotifyIconWithContextMenu();
        _trayManager.SetMenuState_ShowConsole(arguments.ShowConsole);
        _trayManager.SetMenuState_LogToFile(arguments.LogToFile);
    }

    public void DisposeTray()
    {
        _trayManager.DisposeTray();
    }
}
