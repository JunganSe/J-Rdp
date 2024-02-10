using Core.Main;

namespace ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        new PollingController().Run();
    }
}
