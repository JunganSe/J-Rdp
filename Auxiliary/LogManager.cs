using NLog;

namespace Auxiliary;

public static class LogManager
{
    public static void Initialize()
    {
        var assembly = typeof(LogManager).Assembly;
        NLog.LogManager.Setup().LoadConfigurationFromAssemblyResource(assembly);
    }
}
