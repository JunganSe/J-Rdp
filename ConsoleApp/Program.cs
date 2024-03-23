using Core.Main;

namespace ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        var arguments = Arguments.Parse(args);

        if (arguments.HideConsole)
            ConsoleManager.Hide();
        else
            ConsoleManager.Show();
        
        new Controller(arguments.PollingInterval).Run();
    }
}
