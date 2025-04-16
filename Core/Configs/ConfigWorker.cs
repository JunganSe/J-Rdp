﻿using Core.Files;
using NLog;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Core.Configs;

internal class ConfigWorker
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly FileReader _fileReader = new();
    private readonly FileWriter _fileWriter = new();
    private readonly JsonSerializerOptions _jsonOptions;

    public ConfigWorker()
    {
        _jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
        };
    }

    public Config GetConfigFromFile()
    {
        string path = GetConfigFilePath();
        string fileContent = _fileReader.ReadFile(path);
        var config = ParseConfig(fileContent);
        _logger.Debug("Successfully parsed config from file.");
        return config;
    }

    public void UpdateConfigFile(Config config)
    {
        try
        {
            string path = GetConfigFilePath();
            string json = JsonSerializer.Serialize(config, _jsonOptions);
            _fileWriter.WriteFile(path, json);
            _logger.Debug("Successfully updated config file.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to update config file.");
        }
    }

    public bool IsConfigFileFound()
    {
        string path = GetConfigFilePath();
        return File.Exists(path);
    }

    public void OpenConfigFile()
    {
        try
        {
            string path = GetConfigFilePath();
            var process = new ProcessStartInfo(path) { UseShellExecute = true, };
            Process.Start(process);
            _logger.Info("Config file opened in shell.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to open config file.");
        }
    }

    public void CreateConfigFile()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"Core.{ConfigConstants.FileName}";

            using var resourceStream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

            using var fileStream = new FileStream(GetConfigFilePath(), FileMode.CreateNew, FileAccess.Write);
            resourceStream.CopyTo(fileStream);

            _logger.Info($"Created config file.");
        }
        catch (Exception ex)
        {
            string alreadyExistsMessage = (ex is IOException && ex.Message.Contains("already exists"))
                ? $" File already exists." : "";
            _logger.Error(ex, $"Failed to create config file.{alreadyExistsMessage}");
        }
    }

    private string GetConfigFilePath()
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
                ?? throw new InvalidOperationException("Config is null.");

            for (int i = 0; i < config.Profiles.Count; i++)
            {
                var profile = config.Profiles[i];
                profile.Id = i + 1;
                profile.Filter = profile.Filter.Trim();
            }

            return config;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to parse config file.");
            throw;
        }
    }
}