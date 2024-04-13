namespace Core.Configuration;

internal class Config
{
    public int PollingInterval { get; set; }
    public int DeleteDelay { get; set; }
    public IEnumerable<Profile> Profiles { get; set; } = [];
}
