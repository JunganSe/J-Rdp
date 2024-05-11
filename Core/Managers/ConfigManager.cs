using Auxiliary;
using Core.Configuration;
using Core.Constants;
using Core.Extensions;
using Core.Helpers;
using Core.Workers;
using NLog;
using System.Text.Json;

namespace Core.Managers;

internal class ConfigManager
{
    private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly FileReader _fileReader = new();

    public Config Config { get; private set; } = new();

    public ConfigManager()
    {
        _jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }



    public int GetPollingInterval()
        => MathExt.Median(Config.PollingInterval,
                          ConfigConstants.PollingInterval_Min,
                          ConfigConstants.PollingInterval_Max);

    public int GetDeleteDelay()
        => MathExt.Median(Config.DeleteDelay,
                          ConfigConstants.DeleteDelay_Min,
                          ConfigConstants.DeleteDelay_Max);

    public void UpdateConfig()
    {
        try
        {
            var config = GetConfigFromFile();
            config.Profiles.RemoveDisabled();
            LogInvalidProfiles(config.Profiles);
            config.Profiles.RemoveInvalid();
            config.Profiles.AddDefaultFilterFileEndings();
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
        string json = _fileReader.ReadFile(path);
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
