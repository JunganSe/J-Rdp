using NLog;
using System.Reflection;

namespace ConsoleApp;
internal class Arguments
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public int PollingInterval { get; set; }
    public bool Log { get; set; } // TODO: Implement.

    public static Arguments Parse(string[] args)
    {
        try
        {
            var argsDict = GetArgumentsAsDictionary(args);
            var output = new Arguments();
            var properties = output.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (!argsDict.TryGetValue(property.Name, out string? strVal))
                    continue;

                output.SetPropertyValue(property, strVal);
            }

            return output;
        }
        catch (Exception ex)
        {
            _logger.Warn(ex, "Failed to parse arguments.");
            return new Arguments();
        }
    }

    private static Dictionary<string, string> GetArgumentsAsDictionary(string[] args)
    {
        var output = new Dictionary<string, string>();

        foreach (string arg in args)
        {
            var parts = arg.Split('=');
            if (parts.Length != 2 || parts.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"Invalid argument: '{arg}'");

            string key = parts[0];
            string value = parts[1];
            output[key] = value;
        }

        return output;
    }

    private void SetPropertyValue(PropertyInfo property, string strVal)
    {
        if (property.PropertyType == typeof(int) && int.TryParse(strVal, out int i))
            property.SetValue(this, i);
        else if (property.PropertyType == typeof(bool) && bool.TryParse(strVal, out bool b))
            property.SetValue(this, b);
        else if (property.PropertyType == typeof(string))
            property.SetValue(this, strVal);
        else
            return;

        _logger.Trace($"Successfully parsed argument '{property.Name}': {strVal}");
    }
}
