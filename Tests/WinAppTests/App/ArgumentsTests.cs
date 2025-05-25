using WinApp.App;

namespace WinAppTests.App;

[TestClass]
public sealed class ArgumentsTests
{
    [TestMethod]
    [DataRow(ArgumentsExpectation.AllTrue, "-ShowConsole -LogToFile -NoTray")]
    [DataRow(ArgumentsExpectation.AllFalse, "x-ShowConsole x-LogToFile x-NoTray")]
    [DataRow(ArgumentsExpectation.AllFalse, "ShowConsole LogToFile NoTray")]
    [DataRow(ArgumentsExpectation.AllFalse, "Lorem Ipsum Nonsense Arguments")]
    [DataRow(ArgumentsExpectation.AllFalse, "")]
    [DataRow(ArgumentsExpectation.AllFalse, "   ")]
    [DataRow(ArgumentsExpectation.ShowConsoleOnly, "-ShowConsole")]
    [DataRow(ArgumentsExpectation.LogToFileOnly, "-LogToFile")]
    [DataRow(ArgumentsExpectation.NoTrayOnly, "-NoTray")]
    public void Parse_Valid(ArgumentsExpectation expectation, string args)
    {
        // Arrange
        var splitArgs = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Act
        var parsedArguments = Arguments.Parse(splitArgs);

        // Assert
        bool isAsExpected = expectation switch
        {
            ArgumentsExpectation.AllTrue         => parsedArguments is { ShowConsole: true,  LogToFile: true,  NoTray: true },
            ArgumentsExpectation.AllFalse        => parsedArguments is { ShowConsole: false, LogToFile: false, NoTray: false },
            ArgumentsExpectation.ShowConsoleOnly => parsedArguments is { ShowConsole: true,  LogToFile: false, NoTray: false },
            ArgumentsExpectation.LogToFileOnly   => parsedArguments is { ShowConsole: false, LogToFile: true,  NoTray: false },
            ArgumentsExpectation.NoTrayOnly      => parsedArguments is { ShowConsole: false, LogToFile: false, NoTray: true },
            _ => throw new AssertFailedException("Unexpected expectation value.")
        };
        Assert.IsTrue(isAsExpected);
    }

    public enum ArgumentsExpectation
    {
        AllTrue,
        AllFalse,
        ShowConsoleOnly,
        LogToFileOnly,
        NoTrayOnly,
    }
}