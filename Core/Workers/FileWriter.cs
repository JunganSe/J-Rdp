using NLog;

namespace Core.Workers;

internal class FileWriter
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void WriteFile(string path, string content)
    {
        try
        {
            _logger.Trace($"Attempting to write file: '{path}'");

            File.WriteAllText(path, content);

            _logger.Trace($"Successfully wrote file: '{path}'");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to write file: '{path}'");
            throw;
        }
    }
}
