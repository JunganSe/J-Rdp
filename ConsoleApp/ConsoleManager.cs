#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

using NLog;
using System.Runtime.InteropServices;

internal static class ConsoleManager
{
    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow

    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WS_EX_APPWINDOW = 0x00040000;

    private static readonly IntPtr _handle = GetConsoleWindow();
    private static readonly int _windowLong = GetWindowLong(_handle, GWL_EXSTYLE);
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow(); // Gets a handle to a currently connected console window.

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow); // Used to control visibility and state of a window.

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex); // Gets info about a window.

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong); // Used to manipulate window styles.

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
        SetWindowLong(_handle, GWL_EXSTYLE, _windowLong & ~WS_EX_TOOLWINDOW | WS_EX_APPWINDOW); // Remove toolwindow style and add appwindow style to console window.
        ShowWindow(_handle, SW_SHOW);
    }

    private static void Hide()
    {
        _logger.Info("Hiding Console.");
        SetWindowLong(_handle, GWL_EXSTYLE, _windowLong & ~WS_EX_APPWINDOW | WS_EX_TOOLWINDOW); // Remove appwindow style and add toolwindow style to console window.
        ShowWindow(_handle, SW_HIDE);
    }
}
