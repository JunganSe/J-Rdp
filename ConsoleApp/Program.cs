using Auxiliary;
using Core.Main;

namespace ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        Console.Title = "J-Rdp 0.1.0";

        LogManager.Initialize();
        var logger = NLog.LogManager.GetCurrentClassLogger();
        logger.Info("Starting application...");

        var arguments = Arguments.Parse(args);
        ConsoleManager.SetVisibility(!arguments.HideConsole);

        new Controller().Run();

        logger.Info("Quitting application...");
    }
}
