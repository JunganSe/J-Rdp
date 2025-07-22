using Core.Profiles;

namespace Core.Configs;

/// <summary>
/// Represents the config file.
/// </summary>
internal class Config
{
    public int PollingInterval { get; set; }    = ConfigConstants.PollingInterval_Default;
    public int DeleteDelay { get; set; }        = ConfigConstants.DeleteDelay_Default;
    public bool ShowLogConsole { get; set; }    = ConfigConstants.ShowLogConsole_Default;
    public bool LogToFile { get; set; }         = ConfigConstants.LogToFile_Default;
    public List<Profile> Profiles { get; set; } = [];
}
