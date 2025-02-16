using NLog;

namespace Core.Workers;

internal class FileWriter
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void WriteFile(string path, string content)
    {
        try
        {
            File.WriteAllText(path, content);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to write file: '{path}'");
            throw;
        }
    }
}
