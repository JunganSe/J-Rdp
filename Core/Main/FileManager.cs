using System.IO.Enumeration;

namespace Core.Main;

internal class FileManager
{
    public static bool FileNameMatchesFilter(string path, string filter)
    {
        ReadOnlySpan<char> fileName = Path.GetFileName(path);
        return (!fileName.IsEmpty
            && FileSystemName.MatchesSimpleExpression(filter, fileName, ignoreCase: true));
    }

    public static string GetLastExistingFolderName(string path)
    {
        var dir = new DirectoryInfo(path);
        while (!dir.Exists)
            dir = dir.Parent ?? dir.Root;
        return dir.ToString();
    }

    /// <summary>
    /// Returns the name of the first non-existing directory in the path.
    /// Throws ArgumentException if root drive does not exist, or if the full path already exists.
    /// </summary>
    public static string GetFirstMissingFolderName(string path)
    {
        var dir = new DirectoryInfo(path);
        var firstMissingDirectory = GetFirstMissingDirectory(dir);
        return firstMissingDirectory.Name;
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
}
