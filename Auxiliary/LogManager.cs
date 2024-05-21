using NLog;
using NLog.Config;
using NLog.Targets;

namespace Auxiliary;

public class LogManager
{
    private const string _configFileName = "nlog.config";
    private const string _fileRuleName = "file";

    public void Initialize()
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string externalConfigPath = Path.Combine(baseDirectory, _configFileName);

        if (File.Exists(externalConfigPath))
            LoadExternalConfig();
        else
            LoadEmbeddedConfig();
    }

    private void LoadExternalConfig()
    {
        NLog.LogManager.Setup().LoadConfigurationFromFile(_configFileName);
    }

    private void LoadEmbeddedConfig()
    {
        var assembly = typeof(LogManager).Assembly;
        NLog.LogManager.Setup().LoadConfigurationFromAssemblyResource(assembly, _configFileName);
    }

    public void SetFileLogging(bool enabled)
    {
        if (enabled)
            EnableFileLogging();
        else
            DisableFileLogging();
    }

    public void EnableFileLogging()
    {
        var fileRule = GetLoggingRule(_fileRuleName);
        fileRule?.EnableLoggingForLevels(LogLevel.Debug, LogLevel.Fatal);
        NLog.LogManager.ReconfigExistingLoggers();
    }

    public void DisableFileLogging()
    {
        var fileRule = GetLoggingRule(_fileRuleName);
        fileRule?.DisableLoggingForLevels(LogLevel.Trace, LogLevel.Fatal);
        NLog.LogManager.ReconfigExistingLoggers();
    }

    private LoggingRule? GetLoggingRule(string ruleName)
    {
        var config = NLog.LogManager.Configuration;
        var fileTarget = config.FindTargetByName(ruleName) as FileTarget;
        return config.LoggingRules.FirstOrDefault(r => r.Targets.Contains(fileTarget));
    }
}
