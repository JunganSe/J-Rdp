using System.Text.Json;
using Core.Models;
using NLog;

namespace Core.Components;

public class ConfigManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _path;
    private readonly JsonSerializerOptions _jsonOptions;

    public ConfigManager()
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string fileName = "config.json";
        _path = baseDirectory + fileName;
        _jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, };
    }



    public List<Config>? GetConfigs()
    {
        try
        {
            string json = ReadFile();
            return Parse(json);
        }
        catch
        {
            return null;
        }
    }



    private string ReadFile()
    {
        try
        {
            string json = File.ReadAllText(_path);
            _logger.Info("Successfully read file: {path}", _path);
            return json;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to read file: {path}", _path);
            throw;
        }
    }

    private List<Config> Parse(string json)
    {
        try
        {
            var configs = JsonSerializer.Deserialize<List<Config>>(json, _jsonOptions);
            _logger.Info("Successfully parsed configs from json.");
            return configs ?? new List<Config>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to parse configs from json: {json}", json);
            throw;
        }
    }
}
