using WinApp.Managers;
using WinApp.Tray;

namespace WinApp;

internal class Controller
{
    private readonly CoreManager _coreManager = new();
    private readonly TrayManager _trayManager = new();

    public void Run()
    {
        new Core.Controller().Run();
    }

    public void InitializeTray(Arguments arguments)
    {
        _trayManager.InitializeNotifyIconWithContextMenu();
        _trayManager.SetMenuState_ShowConsole(arguments.ShowConsole);
        _trayManager.SetMenuState_LogToFile(arguments.LogToFile);
    }

    public void DisposeTray()
    {
        _trayManager.DisposeTray();
    }
}
