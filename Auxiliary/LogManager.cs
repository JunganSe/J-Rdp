using NLog;
using NLog.Config;
using NLog.Targets;

namespace Auxiliary;

public static class LogManager
{
    private const string _fileRuleName = "file";

    public static void Initialize()
    {
        var assembly = typeof(LogManager).Assembly;
        NLog.LogManager.Setup().LoadConfigurationFromAssemblyResource(assembly);
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
        fileRule?.EnableLoggingForLevels(LogLevel.Trace, LogLevel.Fatal);
        NLog.LogManager.ReconfigExistingLoggers();
    }

    public static void DisableFileLogging()
    {
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
