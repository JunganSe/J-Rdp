using Core.Commands;
using Core.Configs;
using Core.LogDisplay;

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
    public void SetLogDisplayManager(ILogDisplayManager manager)
    {
        var command = new CoreCommand(CoreCommandType.SetLogDisplayManager, manager);
        _coreController?.ExecuteCommand(command);
    }

    /// <summary>
    /// Tell the core controller which method should be called after the config (in memory) has been updated.
    /// </summary>
    public void SetCallback_ConfigUpdated(Handler_OnConfigUpdated callback)
    {
        var command = new CoreCommand(CoreCommandType.SetCallback_ConfigUpdated, callback);
        _coreController?.ExecuteCommand(command);
    }

    public void SetCallback_LogClosed(Action callback)
    {
        var command = new CoreCommand(CoreCommandType.SetCallback_LogClosed, callback);
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

    public void ShowLogDisplay(bool showLogDisplay)
    {
        var command = new CoreCommand(CoreCommandType.ShowLogDisplay, showLogDisplay);
        _coreController?.ExecuteCommand(command);
    }

    public void SetLogToFile(bool logToFile)
    {
        var command = new CoreCommand(CoreCommandType.SetLogToFile, logToFile);
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
