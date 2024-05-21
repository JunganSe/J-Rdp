#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

using NLog;
using System.Runtime.InteropServices;

internal static class ConsoleManager
{
    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow

    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;
    private static readonly IntPtr _handle = GetConsoleWindow();
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static void SetVisibility(bool show)
    {
        if (show)
            Show();
        else
            Hide();
    }

    private static void Show()
    {
        _logger.Info("Showing Console.");
        ShowWindow(_handle, SW_SHOW);
    }

    private static void Hide()
    {
        _logger.Info("Hiding Console.");
        ShowWindow(_handle, SW_HIDE); 
    }
}
