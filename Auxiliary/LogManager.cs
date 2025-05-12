using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;

namespace Auxiliary;

public static class LogManager
{
    private const string _configFileName = "nlog.config";
    private const string _fileRuleName = "file";
    private const string _fileLoggingEnabledVariable = "fileLoggingEnabled";

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

        AddFileRuleVariable();
        AddFileRuleFilter();
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

    public static void SetFileLogging(bool enabled)
    {
        if (enabled)
            EnableFileLogging();
        else
            DisableFileLogging();
    }

    private static void EnableFileLogging()
    {
        NLog.LogManager.Configuration.Variables["fileLoggingEnabled"] = "true";
        NLog.LogManager.ReconfigExistingLoggers();
        _logger.Info("Logging to file enabled.");
    }

    private static void DisableFileLogging()
    {
        _logger.Info("Disabling logging to file.");
        NLog.LogManager.Configuration.Variables["fileLoggingEnabled"] = "false";
        NLog.LogManager.ReconfigExistingLoggers();
    }

    private static void AddFileRuleVariable()
    {
        NLog.LogManager.Configuration.Variables["fileLoggingEnabled"] = "false";
        NLog.LogManager.ReconfigExistingLoggers();
        _logger.Trace("File logging variable added.");
    }

    private static void AddFileRuleFilter()
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
            Condition = "'${fileLoggingEnabled}' == 'true'", // Hittar inte variabeln, det blir {('' == 'true')}
            //Condition = $"'{_fileLoggingEnabled}' == 'true'", // Kör man på denna vägen måste själva filtret uppdateras när man vill ställa loggning på/av
            Action = FilterResult.Log
        };
        fileRule.Filters.Add(filter);
        NLog.LogManager.ReconfigExistingLoggers();
        _logger.Trace("File logging filter added.");
    }

    private static LoggingRule? GetLoggingRule(string ruleName)
    {
        var config = NLog.LogManager.Configuration;
        var fileTarget = config.FindTargetByName(ruleName) as FileTarget;
        return config.LoggingRules.FirstOrDefault(r => r.Targets.Contains(fileTarget));
    }
}
