namespace Core.Configs;

/// <summary>
/// Used to represent config settings when communicating with the UI. <br/>
/// Properties that are null will be ignored.
/// </summary>
public class ConfigInfo
{
    public bool? ShowLogConsole { get; set; }
    public bool? LogToFile { get; set; }
}
