using NLog;
using System.IO.Enumeration;

namespace Core.Helpers;

internal static class FileHelper
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static bool FileNameMatchesFilter(string path, string filter)
    {
        ReadOnlySpan<char> fileName = Path.GetFileName(path);
        return (!fileName.IsEmpty
            && FileSystemName.MatchesSimpleExpression(filter, fileName, ignoreCase: true));
    }

    public static string GetConfigDirectory()
        => AppDomain.CurrentDomain.BaseDirectory;

    public static string ReadFile(string path)
    {
        try
        {
            if (!File.Exists(path))
                throw new ArgumentException("File does not exist.");

            string json = File.ReadAllText(path);
            _logger.Trace($"Successfully read file: {path}");
            return json;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to read file: {path}");
            throw;
        }
    }
}
