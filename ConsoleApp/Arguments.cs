using NLog;

namespace ConsoleApp;

internal class Arguments
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public int PollingInterval { get; set; }
    public bool HideConsole { get; set; }

    public static Arguments Parse(string[] args)
    {
        _logger.Trace("Parsing arguments...");

        var output = new Arguments();
        foreach (var arg in args)
        {
            var parts = arg.Split('=');
            if (parts.Length != 2)
                continue;

            if (parts[0] == nameof(PollingInterval))
                output.PollingInterval = ParseInt(parts[1]);
            else if (parts[0] == nameof(HideConsole))
                output.HideConsole = ParseBool(parts[1]);
        }

        _logger.Debug("Parsed arguments: " + output.ToString());
        return output;
    }

    private static int ParseInt(string input) 
        => (int.TryParse(input, out int result)) ? result : 0;

    private static bool ParseBool(string input) 
        => (bool.TryParse(input, out bool result)) ? result : false;

    public override string ToString()
        => $"{nameof(PollingInterval)}: {PollingInterval}, " +
            $"{nameof(HideConsole)}: {HideConsole}";
}
