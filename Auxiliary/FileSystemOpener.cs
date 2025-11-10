using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Auxiliary;

public static class FileSystemOpener
{
    /// <summary>
    /// Opens a directory in the default file explorer.
    /// </summary>
    public static void OpenDirectory(string path)
    {
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Directory not found: {path}");

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start("explorer.exe", path);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", path);
            else
                throw new PlatformNotSupportedException("Unsupported operating system");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to open directory: {path}", ex);
        }
    }

    /// <summary>
    /// Opens a file with its default associated application.
    /// </summary>
    public static void OpenFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start(new ProcessStartInfo() { FileName = path, UseShellExecute = true });
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", path);
            else
                throw new PlatformNotSupportedException("Unsupported operating system");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to open file: {path}", ex);
        }
    }
}
