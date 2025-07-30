using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;
using System.Data;
using System.Diagnostics;

namespace Auxiliary;

public static class LogManager
{
    private const string _configFileName = "nlog.config";

    private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    #region Initialization

    public static void Initialize()
    {
        _logger.Trace("Initializing LogManager...");
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string externalConfigPath = Path.Combine(baseDirectory, _configFileName);

        if (File.Exists(externalConfigPath))
            LoadExternalConfig();
        else
            LoadEmbeddedConfig();

        SetFileRulesEnabled(false);
        _logger.Debug("Initialized LogManager.");
    }

    private static void LoadExternalConfig()
    {
        NLog.LogManager.Setup().LoadConfigurationFromFile(_configFileName);
        _logger.Debug("Loaded external NLog configuration file.");
    }

    private static void LoadEmbeddedConfig()
    {
        var assembly = typeof(LogManager).Assembly;
        NLog.LogManager.Setup().LoadConfigurationFromAssemblyResource(assembly, _configFileName);
        _logger.Debug("Loaded embedded NLog configuration file.");
    }

    #endregion

    #region File logging

    public static void SetFileLogging(bool enable)
    {
        // Seemingly unnecessary if/else block because of different logging and execution order.
        if (enable)
        {
            SetFileRulesEnabled(true);
            _logger.Info("Enabled logging to file.");
        }
        else
        {
            _logger.Info("Disabling logging to file...");
            SetFileRulesEnabled(false);
            _logger.Info("Disabled logging to file.");
        }
    }

    /// <summary>
    /// Checks whether any filters in the rule are excplicitly set to "true".
    /// </summary>
    private static bool IsRuleEnabled(LoggingRule rule) =>
        rule.Filters.Any(filter =>
            filter is ConditionBasedFilter conditionBasedFilter
            && bool.TryParse(conditionBasedFilter.Condition.ToString(), out bool isConditionTrue)
            && isConditionTrue);

    private static void SetFileRulesEnabled(bool enable)
    {
        var fileRules = GetFileLoggingRules();
        if (fileRules.Count == 0)
        {
            _logger.Error("Cannot set file logging state. No file logging rules found.");
            return;
        }

        foreach (var rule in fileRules)
            SetRuleEnabled(rule, enable);

        NLog.LogManager.ReconfigExistingLoggers();
    }

    private static List<LoggingRule> GetFileLoggingRules()
    {
        return NLog.LogManager
            .Configuration
            .LoggingRules
            .Where(rule => rule.Targets.OfType<FileTarget>().Any())
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

    #endregion


    #region Open logs folder

    public static void OpenLogsFolder()
    {
        var fileRules = GetFileLoggingRules();
        if (fileRules.Count == 0)
        {
            _logger.Error("Cannot open logs folder. No file logging rules found.");
            return;
        }

        foreach (var fileRule in fileRules)
        {
            var fileTargets = fileRule.Targets.OfType<FileTarget>();
            OpenFileTargetsFolders(fileTargets);
        }
    }

    private static void OpenFileTargetsFolders(IEnumerable<FileTarget> fileTargets)
    {
        try
        {
            foreach (var fileTarget in fileTargets)
                OpenFileTargetFolder(fileTarget);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to open logs folder.");
        }
    }

    private static void OpenFileTargetFolder(FileTarget fileTarget)
    {
        string? logDirectory = Path.GetDirectoryName(fileTarget.FileName.ToString());
        if (logDirectory is null)
        {
            _logger.Error("Failed to open logs folder. Logs folder path is missing.");
            return;
        }

        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string fullPath = Path.Combine(baseDirectory, logDirectory);
        if (!Directory.Exists(fullPath))
        {
            _logger.Error($"Failed to open logs folder. Directory does not exist: '{fullPath}'");
            return;
        }

        var process = new ProcessStartInfo(fullPath) { UseShellExecute = true };
        Process.Start(process);
        _logger.Info($"Opened logs folder: '{fullPath}'");
    }

    #endregion
}
