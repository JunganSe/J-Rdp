using Core.Constants;

namespace Core.Models;

internal class Config
{
    public int PollingInterval { get; set; }    = ConfigConstants.PollingInterval_Default;
    public int DeleteDelay { get; set; }        = ConfigConstants.DeleteDelay_Default;
    public List<Profile> Profiles { get; set; } = [];
}
