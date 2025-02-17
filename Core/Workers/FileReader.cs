using Core.Constants;
using NLog;

namespace Core.Workers;

internal class FileReader
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public string ReadFile(string path)
    {
        try
        {
            _logger.Trace($"Attempting to read file: '{path}'");
            if (!File.Exists(path))
                throw new FileNotFoundException("File does not exist.", path);

            string fileContent = ReadFileWithRetries(path);
            _logger.Trace($"Successfully read file: '{path}'");
            return fileContent;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to read file: '{path}'");
            throw;
        }
    }

    private string ReadFileWithRetries(string path)
    {
        int tryCountMax = Math.Max(1, FileConstants.ReadFile_TryCountMax);
        for (int tryCount = 1; tryCount <= tryCountMax; tryCount++)
        {
            try
            {
                return File.ReadAllText(path);
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
