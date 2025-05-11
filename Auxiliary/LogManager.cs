using NLog;

namespace Auxiliary;

public static class LogManager
{
    private const string _configFileName = "nlog.config";
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
        NLog.LogManager.Configuration.Variables[_fileLoggingEnabledVariable] = "true";
        NLog.LogManager.ReconfigExistingLoggers();
        _logger.Info("Logging to file enabled.");
    }

    private static void DisableFileLogging()
    {
        _logger.Info("Disabling logging to file.");
        NLog.LogManager.Configuration.Variables[_fileLoggingEnabledVariable] = "false";
        NLog.LogManager.ReconfigExistingLoggers();
    }
}
