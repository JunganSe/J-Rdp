using NLog;
using System.Runtime.InteropServices;

namespace WinApp.Managers;

/// <summary> Windows exclusive manager for opening and closing a console log window </summary>
internal static class ConsoleManager
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool SetConsoleTitle(string lpConsoleTitle);

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

        SetConsoleTitle("J-Rdp log");
        DisableConsoleCloseButton();
        RegisterCloseEvents();
        RedirectConsoleOutput();

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

    private static void RegisterCloseEvents()
    {
        Console.CancelKeyPress += OnCancelKeyPress;
    }

    private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs eventArgs)
    {
        eventArgs.Cancel = true; // Prevents the console from closing and taking the main app with it.
        CloseConsole();
    }

    private static void RedirectConsoleOutput()
    {
        var consoleOutput = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
        Console.SetOut(consoleOutput);
        Console.SetError(consoleOutput);
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
