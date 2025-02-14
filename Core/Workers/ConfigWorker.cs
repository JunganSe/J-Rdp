using Core.Constants;
using Core.Helpers;
using Core.Models;
using NLog;
using System.Text.Json;

namespace Core.Workers;

internal class ConfigWorker
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly FileReader _fileReader = new();
    private readonly JsonSerializerOptions _jsonOptions;

    public ConfigWorker()
    {
        _jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }

    public Config GetConfigFromFile()
    {
        string path = GetConfigPath();
        string fileContent = _fileReader.ReadFile(path);
        var config = ParseConfig(fileContent);
        _logger.Debug("Successfully parsed config from file.");
        return config;
    }



    private string GetConfigPath()
    {
        string directory = FileHelper.GetConfigDirectory();
        string fileName = ConfigConstants.FileName;
        return Path.Combine(directory, fileName);
    }

    private Config ParseConfig(string json)
    {
        try
        {
            var config = JsonSerializer.Deserialize<Config>(json, _jsonOptions) 
                ?? throw new InvalidOperationException("Config is null.");

            for (int i = 0; i < config.Profiles.Count; i++)
            {
                var profile = config.Profiles[i];
                profile.SetId(i + 1);
                profile.Filter = profile.Filter.Trim();
            }

            return config;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to parse config file.");
            throw;
        }
    }

    // TODO: Method to update config file with profiles as input.
    // Then read the config from file again?
    // Then update the context menu with the new profiles. (Because id assignment on parse.)
}
