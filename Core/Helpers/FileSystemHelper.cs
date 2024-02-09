using System.IO.Enumeration;

namespace Core.Helpers;

internal class FileSystemHelper
{
    public bool FileNameMatchesFilter(FileInfo file, string filter) 
        => FileNameMatchesFilter(file.Name, filter);

    public bool FileNameMatchesFilter(string fileName, string filter) 
        => (!string.IsNullOrEmpty(fileName)
            && FileSystemName.MatchesSimpleExpression(filter, fileName, ignoreCase: true));
}
