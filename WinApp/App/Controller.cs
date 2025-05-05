using WinApp.LogConsole;
using WinApp.Managers;
using WinApp.Tray;

namespace WinApp.App;

internal class Controller
{
    private readonly ConsoleManager _consoleManager = new();
    private readonly TrayManager _trayManager = new();
    private readonly CoreManager _coreManager = new();

    public void Run(Arguments arguments)
    {
        Initialize(arguments); // Initialize on the current thread.
        Task.Run(_coreManager.Run); // Run CoreManager asynchronously, running in parallell on the same thread.
    }

    public void StopCore()
    {
        _coreManager.Stop();
    }



    private void Initialize(Arguments arguments)
    {
        SetConsoleVisibility(arguments.ShowConsole);
        if (!arguments.NoTray)
            InitializeTray(arguments);
        InitializeCore(arguments);
    }

    private void SetConsoleVisibility(bool show)
    {
        _consoleManager.SetVisibility(show);
        _consoleManager.SetCallback_ConsoleClosed(() =>
        {
            _trayManager.SetMenuState_ShowConsole(false);
        });
    }

    private void InitializeTray(Arguments arguments)
    {
        _trayManager.SetCallback_ToggleConsole(_consoleManager.SetVisibility);
        _trayManager.SetCallback_OpenConfigFile(_coreManager.OpenConfigFile);
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

    public void CloseAndDisposeConsole() =>
        _consoleManager.SetVisibility(false);
}
