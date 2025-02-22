using WinApp.Managers;
using WinApp.Tray;

namespace WinApp;

internal class Controller
{
    private readonly CoreManager _coreManager = new();
    private readonly TrayManager _trayManager = new();
}
