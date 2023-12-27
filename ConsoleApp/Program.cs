using Core.Components;

namespace ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        var ui = new Ui();
        var watcher = new Watcher(ui);
        watcher.Start();
    }
}
