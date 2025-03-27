using System.IO.Enumeration;

namespace Core.Files;

internal static class FileHelper
{
    public static bool FileNameMatchesFilter(string path, string filter)
    {
        string fileName = Path.GetFileName(path);
        return FileSystemName.MatchesSimpleExpression(filter, fileName, ignoreCase: true);
    }

    public static bool IsPathAbsolute(string path) =>
        Path.IsPathFullyQualified(path); // e.g. 'C:\Foo\Bar'

    public static string GetConfigDirectory() =>
        AppDomain.CurrentDomain.BaseDirectory;
}
