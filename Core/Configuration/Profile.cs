using Core.Constants;

namespace Core.Configuration;

internal class Profile
{
    public bool Enabled { get; set; }           = true;
    public string Name { get; init; }           = ConfigConstants.Profile_DefaultName;
    public string WatchFolder { get; init; }    = "";
    public string Filter { get; set; }         = "";
    public string MoveToFolder { get; init; }   = "";
    public bool Launch { get; init; }           = false;
    public bool Delete { get; init; }           = false;
    public List<string> Settings { get; init; } = [];
}
