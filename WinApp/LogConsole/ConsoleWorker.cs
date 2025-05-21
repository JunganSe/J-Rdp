using Auxiliary;
using System.Runtime.InteropServices;

namespace WinApp.LogConsole;

/// <summary> Windows exclusive worker for opening and closing a console log window. </summary>
internal partial class ConsoleWorker
{
    private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private Action? _callback_ConsoleClosed;

    #region Windows integration

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AllocConsole();

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool FreeConsole();

    [LibraryImport("kernel32.dll")]
    private static partial nint GetConsoleWindow();

    [LibraryImport("user32.dll")]
    private static partial nint GetSystemMenu(nint hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DeleteMenu(nint hMenu, uint uPosition, uint uFlags);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleCtrlHandler(CtrlTypesHandler handler, [MarshalAs(UnmanagedType.Bool)] bool add);

    private delegate bool CtrlTypesHandler(CtrlTypes ctrlType);

    private enum CtrlTypes
    {
        CTRL_C_EVENT = 0,       // Event raised when the user presses Ctrl+C.
        CTRL_BREAK_EVENT = 1,   // Event raised when the user presses Ctrl+Break.
        CTRL_CLOSE_EVENT = 2,   // Event raised when the user closes the console window.
        CTRL_LOGOFF_EVENT = 5,  // Event raised when the user logs off (only received by services).
        CTRL_SHUTDOWN_EVENT = 6 // Event raised when the system is shutting down (only received by services).
    }

    #endregion



    public void SetCallback_ConsoleClosed(Action callback) =>
        _callback_ConsoleClosed = callback;

    public void AllocateConsole()
    {
        string errorMessage = "Error opening console. Allocation failed.";
        try
        {
            bool isSuccess = AllocConsole();
            if (!isSuccess)
                _logger.Error(errorMessage);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, errorMessage);
        }
    }

    public void SetConsoleTitle()
    {
        try
        {
            var assembly = typeof(WinApp.Program).Assembly;
            string name = AssemblyHelper.GetAssemblyName(assembly);
            string version = AssemblyHelper.GetAssemblyVersion(assembly);
            Console.Title = $"{name} {version} log";
        }
        catch (Exception ex)
        {
            _logger.Warn(ex, "Error setting console title.");
        }
    }

    public void DisableConsoleCloseButton()
    {
        try
        {
            nint consoleWindow = GetConsoleWindow();
            nint systemMenu = GetSystemMenu(consoleWindow, false);
            if (systemMenu != nint.Zero)
                DeleteMenu(systemMenu, 0xF060, 0x00000000);
        }
        catch (Exception ex)
        {
            _logger.Warn(ex, "Error disabling console close button.");
        }
    }

    public void RedirectConsoleOutput()
    {
        try
        {
            var consoleStream = Console.OpenStandardOutput();
            var consoleOutput = new StreamWriter(consoleStream) { AutoFlush = true };
            Console.SetOut(consoleOutput);
            Console.SetError(consoleOutput);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error redirecting console output.");
        }
    }

    public void SetControlHandler()
    {
        try
        {
            CtrlTypesHandler handler = ConsoleCtrlCheck;
            SetConsoleCtrlHandler(handler, true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error setting control handler.");
        }
    }

    private bool ConsoleCtrlCheck(CtrlTypes ctrlType)
    {
        if (ctrlType is CtrlTypes.CTRL_C_EVENT
                     or CtrlTypes.CTRL_BREAK_EVENT
                     or CtrlTypes.CTRL_CLOSE_EVENT)
        {
            CloseConsole();
        }
        return true; // Tell the OS that the event is handled, cancelling the default behavior (e.g. closing the window).
    }

    public void PrintInfoMessage()
    {
        Console.WriteLine("""
            *****************************************************************
            Use the tray menu or press ctrl+C to safely close the log window.
            Closing it directly through Windows will also close the main app.
            *****************************************************************

            """);
    }

    public void CloseConsole()
    {
        try
        {
            bool isSuccess = FreeConsole(); // Close the console without closing the main app.
            if (isSuccess)
            {
                _logger.Info("Closed log console.");
                _callback_ConsoleClosed?.Invoke();
            }
            else
                _logger.Warn("Failed to close log console, or it was already closed.");
        }
        catch (Exception ex)
        {
            _logger.Warn(ex, "Error closing console.");
        }
    }
}
