using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;

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
}
