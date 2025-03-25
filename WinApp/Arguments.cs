using NLog;

namespace WinApp;

internal class Arguments
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public bool ShowConsole { get; private set; }
    public bool LogToFile { get; private set; }
    public bool NoTray { get; private set; }

    private Arguments()
    {
    }

    public static Arguments Parse(string[] args)
    {
        _logger.Trace("Parsing arguments...");

        var output = new Arguments();
        var properties = typeof(Arguments).GetProperties();
        foreach ( var property in properties )
        {
            bool value = args.Any(arg => string.Equals(arg, $"-{property.Name}", StringComparison.OrdinalIgnoreCase));
            property.SetValue(output, value);
        }

        _logger.Debug("Parsed arguments: " + output.ToString());
        return output;
    }

    public override string ToString()
    {
        var propertySummaries = this.GetType()
            .GetProperties()
            .Select(p => $"{p.Name}: {p.GetValue(this)}");
        return string.Join(", ", propertySummaries);
    }
}
