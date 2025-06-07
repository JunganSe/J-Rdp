using NLog;
using NLog.Config;
using NLog.Filters;

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
        var fileRule = GetLoggingRule(_fileRuleName);
        if (fileRule is null)
        {
            _logger.Error($"Failed to open log folder. No file log rule found in nlog config.");
            return;
        }

        if (!IsLogFolderFound())
        {
            _logger.Error($"Failed to open log folder. Folder not found.");
            return;
        }

        // TODO: Open the folder containing the log file.
    }s
}
