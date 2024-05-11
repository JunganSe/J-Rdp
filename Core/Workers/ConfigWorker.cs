using Auxiliary;
using Core.Configuration;
using Core.Constants;
using Core.Managers;

namespace Core.Workers;

internal class ConfigWorker
{
    private readonly ConfigManager _configManager = new();

    public List<Profile> Profiles => _configManager.Config.Profiles;

    public void UpdateConfig()
        => _configManager.UpdateConfig();

    public int GetPollingInterval()
        => MathExt.Median(_configManager.Config.PollingInterval,
                          ConfigConstants.PollingInterval_Min,
                          ConfigConstants.PollingInterval_Max);

    public int GetDeleteDelay()
        => MathExt.Median(_configManager.Config.DeleteDelay,
                          ConfigConstants.DeleteDelay_Min,
                          ConfigConstants.DeleteDelay_Max);
}
