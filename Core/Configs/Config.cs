using Core.Profiles;

namespace Core.Configs;

internal class Config
{
    public int PollingInterval { get; set; }    = ConfigConstants.PollingInterval_Default;
    public int DeleteDelay { get; set; }        = ConfigConstants.DeleteDelay_Default;
    public List<Profile> Profiles { get; set; } = [];
}
