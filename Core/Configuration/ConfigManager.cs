using Core.Constants;
using Core.Extensions;
using Core.Helpers;
using NLog;
using System.Linq;
using System.Text.Json;

namespace Core.Configuration;

internal class ConfigManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly JsonSerializerOptions _jsonOptions;

    public Config Config { get; private set; } = new();

    public ConfigManager()
    {
        _jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }



    public void UpdateConfig()
    {
        try
        {
            var config = GetConfigFromFile();
            config.Profiles.RemoveDisabled();
            LogInvalidProfiles(config.Profiles);
            config.Profiles.RemoveInvalid();
            Config = config;
        }
        catch
        {
            Config = new();
            _logger.Warn("Failed to update config. Reverting to default configuration.");
        }
    }



    private Config GetConfigFromFile()
    {
        string path = GetConfigPath();
        string json = FileHelper.ReadFile(path);
        var config = ParseConfig(json);
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
            return JsonSerializer.Deserialize<Config>(json, _jsonOptions)
                ?? throw new Exception("Config is null.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to parse config from json: {json}");
            throw;
        }
    }

    private void LogInvalidProfiles(IEnumerable<Profile> profiles)
    {
        foreach (var profile in profiles)
        {
            if (!profile.IsValid(out string reason))
                _logger.Warn($"Profile '{profile.Name}' is invalid and will be ignored. Reason: {reason}");
        }
    }
}
