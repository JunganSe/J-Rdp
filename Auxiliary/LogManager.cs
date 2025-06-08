using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;
using System.Diagnostics;

namespace Auxiliary;

public static class LogManager
{
    private const string _configFileName = "nlog.config";

    private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    #region Initialization

    public static void Initialize()
    {
        _logger.Trace("Initializing LogManager.");
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string externalConfigPath = Path.Combine(baseDirectory, _configFileName);

        if (File.Exists(externalConfigPath))
            LoadExternalConfig();
        else
            LoadEmbeddedConfig();

        SetFileRulesEnabled(false);
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

    #endregion

    #region File logging

    public static void SetFileLogging(bool enable)
    {
        if (enable)
        {
            SetFileRulesEnabled(true);
            _logger.Info("Enabled logging to file.");
        }
        else
        {
            _logger.Info("Disabling logging to file.");
            SetFileRulesEnabled(false);
        }
    }

    private static void SetFileRulesEnabled(bool enable)
    {
        var fileRules = GetFileLoggingRules();
        if (fileRules.Count == 0)
        {
            _logger.Warn("Cannot set file logging state. No file logging rules found.");
            return;
        }

        foreach (var rule in fileRules)
            SetRuleEnabled(rule, enable);

        NLog.LogManager.ReconfigExistingLoggers();
    }

    private static List<LoggingRule> GetFileLoggingRules()
    {
        var loggingRules = NLog.LogManager.Configuration.LoggingRules;
        var fileTargets = GetFileTargets();
        return loggingRules
            .Where(rule => rule.Targets.Any(ruleTarget => fileTargets.Contains(ruleTarget)))
            .ToList();
    }

    // Enables or disables the file rule by clearing the filters and adding a new filter. A bit of a hack.
    // Using a filter that read a variable was originally intended, but the variable always came in as an empty string...
    private static void SetRuleEnabled(LoggingRule rule, bool enable)
    {
        rule.FilterDefaultAction = FilterResult.Ignore;
        var filter = new ConditionBasedFilter()
        {
            Condition = (enable) ? "true" : "false",
            Action = FilterResult.Log
        };
        rule.Filters.Clear();
        rule.Filters.Add(filter);
    }

    private static List<FileTarget> GetFileTargets()
    {
        return NLog.LogManager
            .Configuration
            .AllTargets
            .OfType<FileTarget>()
            .ToList();
    }

    #endregion


    #region Open logs folder

    public static void OpenLogsFolder()
    {
        var fileTargets = GetFileTargets();
        if (fileTargets.Count == 0)
        {
            _logger.Error($"Failed to open logs folder. No file logging target found in nlog config.");
            return;
        }

        foreach (var fileTarget in fileTargets)
            OpenLogsFolder(fileTarget);
    }

    private static void OpenLogsFolder(FileTarget fileTarget)
    {
        try
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string logDirectory = Path.GetDirectoryName(fileTarget.FileName.ToString()) ?? "";
            string fullPath = Path.Combine(baseDirectory, logDirectory);

            if (!Directory.Exists(fullPath))
            {
                _logger.Error($"Failed to open logs folder. Directory does not exist: {fullPath}");
                return;
            }

            var process = new ProcessStartInfo(fullPath) { UseShellExecute = true };
            Process.Start(process);
            _logger.Info($"Opened logs folder: {fullPath}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to open logs folder.");
        }
    }

    #endregion
}
