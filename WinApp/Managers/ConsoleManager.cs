using NLog;
using System.Runtime.InteropServices;

namespace WinApp.Managers;

/// <summary> Windows exclusive manager for opening and closing a console log window </summary>
internal static class ConsoleManager
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GetConsoleWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint GetSystemMenu(nint hWnd, bool bRevert);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DeleteMenu(nint hMenu, uint uPosition, uint uFlags);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

    private delegate bool HandlerRoutine(CtrlTypes ctrlType);

    private enum CtrlTypes
    {
        CTRL_C_EVENT = 0,       // Event raised when the user presses Ctrl+C.
        CTRL_BREAK_EVENT = 1,   // Event raised when the user presses Ctrl+Break.
        CTRL_CLOSE_EVENT = 2,   // Event raised when the user closes the console window.
        CTRL_LOGOFF_EVENT = 5,  // Event raised when the user logs off (only received by services).
        CTRL_SHUTDOWN_EVENT = 6 // Event raised when the system is shutting down (only received by services).
    }



    public static void SetVisibility(bool show)
    {
        if (show)
            OpenConsole();
        else
            CloseConsole();
    }

    private static void OpenConsole()
    {
        bool isSuccess = AllocConsole();
        if (!isSuccess)
        {
            _logger.Warn("Failed to open console.");
            return;
        }

        Console.Title = "J-Rdp log";
        DisableConsoleCloseButton();
        RedirectConsoleOutput();
        SetConsoleCtrlHandler(ConsoleCtrlCheck, true);

        Console.WriteLine("""
            *****************************************************************
            Use the tray menu or press ctrl+C to safely close the log window.
            Closing it directly through Windows will also close the main app.
            *****************************************************************

            """);
        _logger.Info("Opened log console.");
    }

    private static void DisableConsoleCloseButton()
    {
        nint consoleWindow = GetConsoleWindow();
        nint systemMenu = GetSystemMenu(consoleWindow, false);
        if (systemMenu != nint.Zero)
            DeleteMenu(systemMenu, 0xF060, 0x00000000);
    }

    private static void RedirectConsoleOutput()
    {
        var consoleOutput = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
        Console.SetOut(consoleOutput);
        Console.SetError(consoleOutput);
    }

    private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
    {
        if (ctrlType is CtrlTypes.CTRL_C_EVENT
                     or CtrlTypes.CTRL_BREAK_EVENT
                     or CtrlTypes.CTRL_CLOSE_EVENT)
        {
            // TODO: Update the menu item.
            CloseConsole();
        }
        return true; // Tell the OS that the event is handled, cancelling the default behavior (e.g. closing the window).
    }

    private static void CloseConsole()
    {
        bool isSuccess = FreeConsole(); // Close the console without closing the main app.
        if (isSuccess)
            _logger.Info("Closed log console.");
        else
            _logger.Warn("Failed to close log console.");
    }
}
