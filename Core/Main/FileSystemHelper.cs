using NLog;
using System.IO.Enumeration;

namespace Core.Main;

internal class FileSystemHelper
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static bool FileNameMatchesFilter(string path, string filter)
    {
        ReadOnlySpan<char> fileName = Path.GetFileName(path);
        return (!fileName.IsEmpty
            && FileSystemName.MatchesSimpleExpression(filter, fileName, ignoreCase: true));
    }

    public static string GetLastExistingFolderPath(string path)
    {
        var dir = new DirectoryInfo(path);
        while (!dir.Exists)
            dir = dir.Parent ?? dir.Root;
        return dir.ToString();
    }

    /// <summary>
    /// Returns the name of the first non-existing directory in the path, or null if such directory could not be found.
    /// </summary>
    public static string? GetFirstMissingFolderName(string path)
    {
        try
        {
            var dir = new DirectoryInfo(path);
            return GetFirstMissingDirectory(dir).Name;
        }
        catch (Exception ex)
        {
            _logger.Warn(ex, "Failed to get first missing folder from path: {path}", path);
            return null;
        }
    }

    /// <summary>
    /// Returns the first non-existing directory in the path of targetDirectory.
    /// Throws ArgumentException if root drive does not exist, or if targetDirectory already exists.
    /// </summary>
    public static DirectoryInfo GetFirstMissingDirectory(DirectoryInfo targetDirectory)
    {
        if (!targetDirectory.Root.Exists)
            throw new ArgumentException($"Drive '{targetDirectory.Root}' does not exist.");
        if (targetDirectory.Exists)
            throw new ArgumentException($"Directory already exists: {targetDirectory.FullName}");

        var currentDir = targetDirectory;
        while (!currentDir.Exists)
        {
            var parent = currentDir.Parent ?? currentDir.Root;
            if (parent.Exists)
                return currentDir;
            currentDir = parent;
        }

        // Program should never reach this part, but it's needed to prevent compiler error CS0161: "not all code paths returns a value".
        throw new Exception($"Unexpected state in {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
    }

    public static string CombineAndNormalizePaths(string path1, string path2) 
        => Path.Combine(path1, path2).Replace("/", "\\");
}
