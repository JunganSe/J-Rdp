namespace Core.Configuration;

internal class Config
{
    public string Name { get; init; }           = "";
    public string WatchFolder { get; init; }    = "";
    public string Filter { get; init; }         = "";
    public string MoveToFolder { get; init; }   = "";
    public bool Launch { get; init; }           = false;
    public bool Delete { get; init; }           = false;
    public List<string> Settings { get; init; } = [];
}
