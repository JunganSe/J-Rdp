using Core.Commands;
using Core.Configs;
using Core.LogDisplay;

namespace WinApp.CoreHandling;

internal class CoreManager
{
    private Core.Controller? _coreController;

    /// <summary>
    /// Initialize the core controller.
    /// </summary>
    public void Initialize(Handler_OnConfigUpdated callback_ConfigUpdated,
                           ILogDisplayManager logDisplayManager,
                           Action callback_LogClosed) =>
        _coreController = new(callback_ConfigUpdated, logDisplayManager, callback_LogClosed);

    public void Run() =>
        _coreController?.Run();

    public void Stop() =>
        _coreController?.Stop();

    /// <summary>
    /// Tell the core controller to open the folder containing the log files, if such a rule exists.
    /// </summary>
    public void OpenLogsFolder()
    {
        var command = new CoreCommand(CoreCommandType.OpenLogsFolder);
        _coreController?.ExecuteCommand(command);
    }

    /// <summary>
    /// Tell the core controller to show the log display.
    /// </summary>
    public void ShowLog(bool showLog)
    {
        var command = new CoreCommand(CoreCommandType.ShowLog, showLog);
        _coreController?.ExecuteCommand(command);
    }

    /// <summary>
    /// Tell the core controller to enable or disable logging to file.
    /// </summary>
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
