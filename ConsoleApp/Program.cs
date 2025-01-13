using Auxiliary;
using Core.Main;
using System.Threading;

namespace ConsoleApp;

internal class Program
{
    private static Mutex? _mutex;

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

        if (IsProgramRunning())
        {
            logger.Warn("An instance of the program is already running. Quitting application...");
            Environment.Exit(0);
        }

        logger.Info("Starting application...");
        new Controller().Run();

        logger.Info("Quitting application...");
    }

    private static string GetTitle()
    {
        var type = typeof(Program);
        string name = AssemblyHelper.GetAssemblyName(type);
        string version = AssemblyHelper.GetAssemblyVersion(type);
        return $"{name} {version}";
    }

    private static bool IsProgramRunning()
    {
        const string mutexName = "J-Rdp.UniqueInstance";
        _mutex = new Mutex(true, mutexName, out bool isNewInstance);
        return !isNewInstance;
    }
}
