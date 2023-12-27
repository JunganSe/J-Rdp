using Core.Interfaces;

namespace ConsoleApp;

public class Ui : IUi
{
    public void Print(string text) => 
        Console.WriteLine(text);
}
