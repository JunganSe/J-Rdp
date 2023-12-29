using Core.Main;

namespace ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        var ui = new Ui();
        var controller = new Controller(ui);
        controller.Start();
    }
}
