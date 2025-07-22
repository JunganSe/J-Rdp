using WinApp.App;

namespace WinAppTests.App;

[TestClass]
public sealed class BooleanArgumentsParserTests
{
    [TestMethod]
    [DataRow(ArgumentsExpectation.AllTrue, "-NoTray")]
    [DataRow(ArgumentsExpectation.AllTrue, "aaa -NoTray bbb")]
    [DataRow(ArgumentsExpectation.AllFalse, "x-NoTray")]
    [DataRow(ArgumentsExpectation.AllFalse, "NoTray")]
    [DataRow(ArgumentsExpectation.AllFalse, "Lorem Ipsum Nonsense Arguments")]
    [DataRow(ArgumentsExpectation.AllFalse, "")]
    [DataRow(ArgumentsExpectation.AllFalse, "   ")]
    public void Parse_Valid(ArgumentsExpectation expectation, string args)
    {
        // Arrange
        string[] splitArgs = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Act
        var parsedArguments = BooleanArgumentsParser.Parse<Arguments>(splitArgs);

        // Assert
        bool isAsExpected = expectation switch
        {
            ArgumentsExpectation.AllTrue  => parsedArguments is { NoTray: true },
            ArgumentsExpectation.AllFalse => parsedArguments is { NoTray: false },
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