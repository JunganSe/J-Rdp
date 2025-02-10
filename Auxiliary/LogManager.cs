using NLog;
using NLog.Config;
using NLog.Targets;

namespace Auxiliary;

public static class LogManager
{
    private const string _configFileName = "nlog.config";
    private const string _fileRuleName = "file";

    private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public static void Initialize()
    {
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
    }

    private static void LoadEmbeddedConfig()
    {
        var assembly = typeof(LogManager).Assembly;
        NLog.LogManager.Setup().LoadConfigurationFromAssemblyResource(assembly, _configFileName);
    }

    public static void SetFileLogging(bool enabled)
    {
        if (enabled)
            EnableFileLogging();
        else
            DisableFileLogging();
    }

    public static void EnableFileLogging()
    {
        var fileRule = GetLoggingRule(_fileRuleName);
        fileRule?.EnableLoggingForLevels(LogLevel.Debug, LogLevel.Fatal);
        NLog.LogManager.ReconfigExistingLoggers();
        _logger.Info("Logging to file enabled.");
    }

    public static void DisableFileLogging()
    {
        _logger.Info("Disabling logging to file.");
        var fileRule = GetLoggingRule(_fileRuleName);
        fileRule?.DisableLoggingForLevels(LogLevel.Trace, LogLevel.Fatal);
        NLog.LogManager.ReconfigExistingLoggers();
    }

    private static LoggingRule? GetLoggingRule(string ruleName)
    {
        var config = NLog.LogManager.Configuration;
        var fileTarget = config.FindTargetByName(ruleName) as FileTarget;
        return config.LoggingRules.FirstOrDefault(r => r.Targets.Contains(fileTarget));
    }
}
