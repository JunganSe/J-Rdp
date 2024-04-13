using Core.Constants;
using Core.Helpers;
using NLog;
using System.Text.Json;

namespace Core.Configuration;

internal class ConfigManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly JsonSerializerOptions _jsonOptions;

    public List<Profile> Profiles { get; private set; } = [];

    public ConfigManager()
    {
        _jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }



    public void UpdateProfiles()
    {
        var profiles = GetProfilesFromFile();
        var invalidProfiles = profiles.Where(c => !IsProfileValid(c));
        foreach (var invalidProfile in invalidProfiles)
            _logger.Warn($"Profile '{invalidProfile.Name}' is invalid and will be ignored.");

        Profiles = profiles.Except(invalidProfiles).ToList();
    }



    private List<Profile> GetProfilesFromFile()
    {
        try
        {
            string path = GetConfigPath();
            string json = FileHelper.ReadFile(path);
            var config = ParseConfig(json);
            return config.Profiles.ToList();
        }
        catch
        {
            return [];
        }
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
                ?? throw new Exception();
            _logger.Trace("Successfully parsed config from json.");
            return config;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to parse config from json: {json}");
            throw;
        }
    }

    private bool IsProfileValid(Profile profile)
        => !string.IsNullOrWhiteSpace(profile.WatchFolder);
}
