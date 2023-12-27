namespace Core.Models;

public class Config
{
    public string Name { get; set; }
    public string WatchFolder { get; init; }
    public string WatchFile { get; init; }
    public string MoveToFolder { get; init; }
    public bool Delete { get; init; }
    public Dictionary<string, string> Settings { get; init; }

    public Config()
    {
        Name = "";
        WatchFolder = "";
        WatchFile = "";
        MoveToFolder = "";
        Delete = false;
        Settings = [];
    }
}
