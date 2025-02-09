using NLog;
using System.Runtime.InteropServices;

namespace WinApp;

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
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);



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
            *****************************************************
            Press ctrl+C to safely close the log console window.
            Closing it from Windows will also close the main app.
            *****************************************************

            """);
        _logger.Info("Opened console.");
    }

    private static void DisableConsoleCloseButton()
    {
        IntPtr consoleWindow = GetConsoleWindow();
        IntPtr systemMenu = GetSystemMenu(consoleWindow, false);
        if (systemMenu != IntPtr.Zero)
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
            _logger.Info("Closed console.");
        else
            _logger.Warn("Failed to close console.");
    }
}
