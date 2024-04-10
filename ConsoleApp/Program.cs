using Auxiliary;
using Core.Main;

namespace ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        LogManager.Initialize();
        var logger = NLog.LogManager.GetCurrentClassLogger();
        logger.Info("Starting...");

        var arguments = Arguments.Parse(args);
        ConsoleManager.SetVisibility(!arguments.HideConsole);

        new Controller().Run(arguments.PollingInterval);
        logger.Info("Exiting...");
    }
}
