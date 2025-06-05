using NLog;

namespace WinApp.App;

internal static class BooleanArgumentsParser
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static T Parse<T>(string[] args) where T : new()
    {
        _logger.Trace("Parsing arguments...");

        var output = new T();
        var properties = typeof(T)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(bool));
        foreach (var property in properties)
        {
            bool value = args.Any(arg => string.Equals(arg, $"-{property.Name}", StringComparison.OrdinalIgnoreCase));
            property.SetValue(output, value);
        }

        string summary = GetSummary(output);
        _logger.Debug($"Parsed arguments: {summary}");

        return output;
    }

    public static string GetSummary<T>(T arguments)
    {
        var propertySummaries = typeof(T)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(bool))
            .Select(p => $"{p.Name}: {p.GetValue(arguments)}");
        return string.Join(", ", propertySummaries);
    }
}
