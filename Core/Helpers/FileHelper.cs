using Core.Constants;
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

    public static bool IsPathAbsolute(string path)
        => Path.IsPathFullyQualified(path); // e.g. 'C:\Foo\Bar'

    public static string GetConfigDirectory()
        => AppDomain.CurrentDomain.BaseDirectory;

    public static string ReadFile(string path)
    {
        try
        {
            return ReadFileWithRetries(path);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to read file: {path}");
            throw;
        }
    }

    private static string ReadFileWithRetries(string path)
    {
        _logger.Trace($"Attempting to read file: {path}");

        if (!File.Exists(path))
            throw new ArgumentException("File does not exist.");

        int tryCountMax = FileConstants.ReadFile_TryCountMax;
        for (int tryCount = 1; tryCount <= tryCountMax; tryCount++)
        {
            try
            {
                string json = File.ReadAllText(path);
                _logger.Trace($"Successfully read file: {path}");
                return json;
            }
            catch (IOException ex)
            {
                if (tryCount >= tryCountMax)
                    throw;

                int delay = FileConstants.ReadFile_RetryDelay;
                _logger.Trace($"Failed to read file on attempt {tryCount} of {tryCountMax}. Retrying in {delay} ms. Reason: \"{ex.Message}\"");
                Thread.Sleep(delay);
            }
        }

        throw new InvalidOperationException("Unexpected control flow reached."); // This line should never be reached.
    }
}
