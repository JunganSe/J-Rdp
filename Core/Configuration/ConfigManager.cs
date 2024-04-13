﻿using Core.Constants;
using Core.Helpers;
using NLog;
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
        var config = GetConfigFromFile();
        config.Profiles = GetValidProfiles(config.Profiles).ToList();
        Config = config;
    }



    private Config GetConfigFromFile()
    {
        string path = GetConfigPath();
        string json = FileHelper.ReadFile(path);
        return ParseConfig(json);
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

    private IEnumerable<Profile> GetValidProfiles(IEnumerable<Profile> profiles)
    {
        var invalidProfiles = profiles.Where(c => !IsProfileValid(c));
        foreach (var invalidProfile in invalidProfiles)
            _logger.Warn($"Profile '{invalidProfile.Name}' is invalid and will be ignored.");
        return profiles.Except(invalidProfiles);
    }

    private bool IsProfileValid(Profile profile)
        => !string.IsNullOrWhiteSpace(profile.WatchFolder);
}
