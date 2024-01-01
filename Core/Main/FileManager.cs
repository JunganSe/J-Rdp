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
}
