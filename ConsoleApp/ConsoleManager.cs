using NLog;
using System.Runtime.InteropServices;

static class ConsoleManager
{
    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;
    private static readonly IntPtr _handle = GetConsoleWindow();
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static void Hide()
    {
        _logger.Info("Showing Console.");
        ShowWindow(_handle, SW_HIDE); // Hide the console.
    }

    public static void Show()
    {
        _logger.Info("Hiding Console.");
        ShowWindow(_handle, SW_SHOW); // Show the console.
    }
}