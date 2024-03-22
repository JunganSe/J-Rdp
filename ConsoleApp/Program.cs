using Core.Main;

namespace ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        var arguments = Arguments.Parse(args);

        new Controller(arguments.PollingInterval).Run();
    }
}
