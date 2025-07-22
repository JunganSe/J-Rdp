using Core.Configs;
using Core.Profiles;

namespace WinApp.CoreHandling;

internal class CoreManager
{
    private Core.Controller? _coreController;

    public void Initialize() =>
        _coreController = new();

    public void Run() =>
        _coreController?.Run();

    public void Stop() =>
        _coreController?.Stop();

    /// <summary>
    /// Tell the core controller which method should be called after the config (in memory) has been updated.
    /// </summary>
    public void SetCallback_ConfigUpdated(Handler_OnConfigUpdated callback) =>
        _coreController?.SetCallback_ConfigUpdated(callback);

    /// <summary>
    /// Tell the core controller to open the folder containing the log files, if such a rule exists.
    /// </summary>
    public void OpenLogsFolder() =>
        _coreController?.OpenLogsFolder();

    /// <summary>
    /// Tell the core controller to open the config file in the default editor.
    /// </summary>
    public void OpenConfigFile() =>
        _coreController?.OpenConfigFile();

    /// <summary>
    /// Tell the core controller to update the config (in file and memory).
    /// </summary>
    public void UpdateConfig(ConfigInfo configInfo)
    {
        _coreController?.UpdateConfig(configInfo);
    }
}
