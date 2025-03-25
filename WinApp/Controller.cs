using WinApp.LogConsole;
using WinApp.Managers;
using WinApp.Tray;

namespace WinApp;

internal class Controller
{
    private readonly ConsoleManager _consoleManager = new();
    private readonly TrayManager _trayManager = new();
    private readonly CoreManager _coreManager = new();

    public void Run(Arguments arguments)
    {
        Initialize(arguments); // Initialize on the current thread.
        Task.Run(_coreManager.Run); // Run CoreManager on a new thread.
    }

    private void Initialize(Arguments arguments)
    {
        _consoleManager.SetCallback_ConsoleClosed(() => _trayManager.SetMenuState_ShowConsole(false));
        _consoleManager.SetVisibility(arguments.ShowConsole);
        if (!arguments.NoTray)
            InitializeTray(arguments);
        InitializeCore(arguments);
    }

    private void InitializeTray(Arguments arguments)
    {
        _trayManager.SetCallback_ToggleConsole(_consoleManager.SetVisibility);
        _trayManager.SetCallback_ProfilesActiveStateChanged(_coreManager.UpdateProfilesEnabledState);
        _trayManager.InitializeNotifyIconWithContextMenu();
        _trayManager.SetMenuState_ShowConsole(arguments.ShowConsole);
        _trayManager.SetMenuState_LogToFile(arguments.LogToFile);
    }

    private void InitializeCore(Arguments arguments)
    {
        _coreManager.Initialize();
        if (!arguments.NoTray)
            _coreManager.SetCallback_ConfigUpdated(_trayManager.UpdateMenuProfiles);
    }

    public void DisposeTray() =>
        _trayManager.DisposeTray();
}
