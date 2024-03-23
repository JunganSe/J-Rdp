using System.Runtime.InteropServices;

static class ConsoleManager
{
    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;
    private static readonly IntPtr _handle = GetConsoleWindow();

    [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static void Hide() => ShowWindow(_handle, SW_HIDE); //hide the console
    public static void Show() => ShowWindow(_handle, SW_SHOW); //show the console
}