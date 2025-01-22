#pragma warning disable IDE0052 // Remove unread private members

using Auxiliary;
using Core;
using NLog;

namespace ConsoleApp;

internal class Program
{
    private static Mutex? _mutex; // Intentionally stored in field to keep it in memory.
    private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public static void Main(string[] args)
    {
        RegisterCloseEvents();
        Console.Title = GetTitle();

        var logManager = new Auxiliary.LogManager();
        logManager.Initialize();

        _logger.Trace("Initializing application...");
        var arguments = Arguments.Parse(args);
        logManager.SetFileLogging(arguments.LogToFile);
        ConsoleManager.SetVisibility(!arguments.HideConsole);

        if (IsProgramRunning())
        {
            _logger.Warn("An instance of the program is already running. Closing application.");
            Environment.Exit(0);
        }

        _logger.Info("Starting application.");
        new Controller().Run();

        _logger.Info("Closing application.");
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

    private static void RegisterCloseEvents()
    {
        AppDomain.CurrentDomain.ProcessExit += OnClose;
    }

    private static void OnClose(object? sender, EventArgs eventArgs)
    {
        _logger.Info("Closing application by request.");
    }
}
