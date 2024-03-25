using Core.Helpers;
using NLog;
using System.Text.Json;

namespace Core.Configuration;

internal class ConfigManager
{
    public const string CONFIG_FILE_NAME = "config.json";

    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly JsonSerializerOptions _jsonOptions;

    public List<Config> Configs { get; private set; } = [];

    public ConfigManager()
    {
        _jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }



    public void UpdateConfigs()
    {
        var configs = GetConfigsFromFile();
        var invalidConfigs = configs.Where(c => !IsConfigValid(c));
        foreach (var invalidConfig in invalidConfigs)
            _logger.Warn($"Config '{invalidConfig.Name}' is invalid and will be ignored.");

        Configs = configs.Except(invalidConfigs).ToList();
    }

    public List<Config> GetConfigsFromFile()
    {
        try
        {
            string path = GetConfigPath();
            string json = ReadFile(path);
            return Parse(json);
        }
        catch
        {
            return [];
        }
    }



    private string GetConfigPath()
    {
        string directory = FileSystemHelper.GetConfigDirectory();
        return Path.Combine(directory, CONFIG_FILE_NAME);
    }

    private string ReadFile(string path)
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

    private List<Config> Parse(string json)
    {
        try
        {
            var configs = JsonSerializer.Deserialize<List<Config>>(json, _jsonOptions);
            _logger.Trace("Successfully parsed configs from json.");
            return configs ?? new List<Config>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to parse configs from json: {json}");
            throw;
        }
    }

    private bool IsConfigValid(Config config)
        => !string.IsNullOrWhiteSpace(config.WatchFolder);
}
