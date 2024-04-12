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
            string json = FileSystemHelper.ReadFile(path);
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
        string fileName = ConfigConstants.FileName;
        return Path.Combine(directory, fileName);
    }

    private List<Profile> Parse(string json)
    {
        try
        {
            var profiles = JsonSerializer.Deserialize<List<Profile>>(json, _jsonOptions);
            _logger.Trace("Successfully parsed profiles from json.");
            return profiles ?? new List<Profile>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to parse profiles from json: {json}");
            throw;
        }
    }

    private bool IsProfileValid(Profile profile)
        => !string.IsNullOrWhiteSpace(profile.WatchFolder);
}
