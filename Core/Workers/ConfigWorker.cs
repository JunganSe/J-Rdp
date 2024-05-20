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
        _logger.Info("Successfully parsed config from file.");
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
                ?? throw new Exception("Config is null.");
            config.Profiles.ForEach(p => p.Filter = p.Filter.Trim());
            return config;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to parse config from json: {json}");
            throw;
        }
    }
}
