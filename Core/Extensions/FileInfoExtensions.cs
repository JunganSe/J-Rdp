using System.IO.Enumeration;

namespace Core.Extensions;

internal static class FileInfoExtensions
{
    public static bool NameMatchesFilter(this FileInfo file, string filter, bool ignoreCase = true)
        => FileSystemName.MatchesSimpleExpression(filter, file.Name, ignoreCase);
}
