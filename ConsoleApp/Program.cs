using Auxiliary;
using Core.Main;
using System.Reflection;

namespace ConsoleApp;

internal class Program
{
    public static void Main(string[] args)
    {
        Console.Title = GetTitle();

        var logManager = new LogManager();
        logManager.Initialize();
        logManager.DisableFileLogging();

        var logger = NLog.LogManager.GetCurrentClassLogger();
        logger.Trace("Initializing application...");

        var arguments = Arguments.Parse(args);
        ConsoleManager.SetVisibility(!arguments.HideConsole);
        logManager.SetFileLogging(arguments.LogToFile);

        logger.Info("Starting application...");
        new Controller().Run();

        logger.Info("Quitting application...");
    }

    private static string GetTitle()
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName();
        string name = assemblyName.Name ?? "";
        string longVersion = assemblyName.Version?.ToString() ?? "0.0.0.0";
        string version = longVersion[..longVersion.LastIndexOf('.')];
        return $"{name} {version}";
    }
}
