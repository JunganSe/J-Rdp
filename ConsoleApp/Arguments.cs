namespace ConsoleApp;

internal class Arguments
{
    public int PollingInterval { get; set; }
    public int LogLevel { get; set; } // TODO: Implement. 0: No logging, 1: Messages, 2: Messages + stack trace.

    public static Arguments Parse(string[] args)
    {
        var output = new Arguments();
        foreach (var arg in args)
        {
            var parts = arg.Split('=');
            if (parts.Length != 2)
                continue;

            if (parts[0] == "PollingInterval")
                output.PollingInterval = ParseInt(parts[1]);
            else if (parts[0] == "LogLevel")
                output.LogLevel = ParseInt(parts[1]);
        }
        return output;
    }

    private static int ParseInt(string input) 
        => (int.TryParse(input, out int result)) ? result : 0;
}
