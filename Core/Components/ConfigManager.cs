using System.Text.Json;
using Core.Models;
using NLog;

namespace Core.Components;

public class ConfigManager
{
    private static readonly Lazy<ConfigManager> _instance = new(() => new ConfigManager());
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly string _path;
    private readonly JsonSerializerOptions _jsonOptions;

    private ConfigManager()
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string fileName = "config.json";
        _path = $"{baseDirectory}/{fileName}";
        _jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, };
    }



    public static ConfigManager Instance => _instance.Value;

    public List<Config> GetConfigs()
    {
        string json = ReadFile();
        return Parse(json);
    }



    private string ReadFile()
    {
        try
        {
            return File.ReadAllText(_path);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to read config file at {path}", _path);
            return "";
        }
    }

    private List<Config> Parse(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<Config>>(json, _jsonOptions)
                ?? new List<Config>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to parse configurations from json string: {json}", json);
            return new List<Config>();
        }
    }
}
