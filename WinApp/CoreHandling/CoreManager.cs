using Core.Commands;
using Core.Configs;

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
    public void SetCallback_ConfigUpdated(Handler_OnConfigUpdated callback)
    {
        var command = new CoreCommand(CoreCommandType.SetCallback_ConfigUpdated, callback);
        _coreController?.ExecuteCommand(command);
    }

    /// <summary>
    /// Tell the core controller to open the folder containing the log files, if such a rule exists.
    /// </summary>
    public void OpenLogsFolder()
    {
        var command = new CoreCommand(CoreCommandType.OpenLogsFolder);
        _coreController?.ExecuteCommand(command);
    }

    /// <summary>
    /// Tell the core controller to open the config file in the default editor.
    /// </summary>
    public void OpenConfigFile()
    {
        var command = new CoreCommand(CoreCommandType.OpenConfigFile);
        _coreController?.ExecuteCommand(command);
    }

    /// <summary>
    /// Tell the core controller to update the config (in file and memory).
    /// </summary>
    public void UpdateConfig(ConfigInfo configInfo)
    {
        var command = new CoreCommand(CoreCommandType.UpdateConfig, configInfo);
        _coreController?.ExecuteCommand(command);
    }
}
