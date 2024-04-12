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
        return Path.Combine(directory, ConfigConstants.FileName);
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
