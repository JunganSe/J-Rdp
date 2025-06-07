using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;
using System.Diagnostics;
using System.IO;

namespace Auxiliary;

public static class LogManager
{
    private const string _configFileName = "nlog.config";
    private const string _fileRuleName = "file";

    private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public static void Initialize()
    {
        _logger.Trace("Initializing LogManager.");
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string externalConfigPath = Path.Combine(baseDirectory, _configFileName);

        if (File.Exists(externalConfigPath))
            LoadExternalConfig();
        else
            LoadEmbeddedConfig();

        SetFileRuleEnabled(false);
    }

    private static void LoadExternalConfig()
    {
        NLog.LogManager.Setup().LoadConfigurationFromFile(_configFileName);
        _logger.Trace("Loaded external NLog configuration file.");
    }

    private static void LoadEmbeddedConfig()
    {
        var assembly = typeof(LogManager).Assembly;
        NLog.LogManager.Setup().LoadConfigurationFromAssemblyResource(assembly, _configFileName);
        _logger.Trace("Loaded embedded NLog configuration file.");
    }

    public static void SetFileLogging(bool enable)
    {
        if (enable)
        {
            SetFileRuleEnabled(true);
            _logger.Info("Logging to file enabled.");
        }
        else
        {
            _logger.Info("Disabling logging to file.");
            SetFileRuleEnabled(false);
        }
    }

    // Enables or disables the file rule by clearing the filters and adding a new filter. A bit of a hack.
    // Using a filter that read a variable was originally intended, but the variable always came in as an empty string...
    private static void SetFileRuleEnabled(bool enable)
    {
        var fileRule = GetLoggingRule(_fileRuleName);
        if (fileRule is null)
        {
            _logger.Warn($"Can not add file logging filter. Logging rule '{_fileRuleName}' not found.");
            return;
        }

        fileRule.FilterDefaultAction = FilterResult.Ignore;
        var filter = new ConditionBasedFilter()
        {
            Condition = (enable) ? "true" : "false",
            Action = FilterResult.Log
        };
        fileRule.Filters.Clear();
        fileRule.Filters.Add(filter);
        NLog.LogManager.ReconfigExistingLoggers();
    }

    private static LoggingRule? GetLoggingRule(string ruleName)
    {
        var config = NLog.LogManager.Configuration;
        var fileTarget = config.FindTargetByName(ruleName);
        return config.LoggingRules.FirstOrDefault(rule => rule.Targets.Contains(fileTarget));
    }



    public static void OpenLogFolder()
    {
        var fileTargets = GetFileTargets();
        if (fileTargets.Count == 0)
        {
            _logger.Error($"Failed to open log folder. No file logging target found in nlog config.");
            return;
        }

        foreach (var fileTarget in fileTargets)
            OpenLogFolder(fileTarget);
    }

    private static List<FileTarget> GetFileTargets()
    {
        return NLog.LogManager
            .Configuration
            .AllTargets
            .OfType<FileTarget>()
            .ToList();
    }

    private static void OpenLogFolder(FileTarget fileTarget)
    {
        try
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string logDirectory = Path.GetDirectoryName(fileTarget.FileName.ToString()) ?? "";
            string fullPath = Path.Combine(baseDirectory, logDirectory);

            if (!Directory.Exists(fullPath))
            {
                _logger.Error($"Failed to open log folder. Directory does not exist: {logDirectory}");
                return;
            }

            var process = new ProcessStartInfo(logDirectory) { UseShellExecute = true };
            Process.Start(process);
            _logger.Info($"Opened log folder: {logDirectory}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to open log folder.");
        }
    }
}
