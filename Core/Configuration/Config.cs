using Core.Constants;

namespace Core.Configuration;

internal class Config
{
    public int PollingInterval { get; set; } = ConfigConstants.PollingInterval_Default;
    public int DeleteDelay { get; set; } = ConfigConstants.DeleteDelay_Default;
    public IEnumerable<Profile> Profiles { get; set; } = [];
}
