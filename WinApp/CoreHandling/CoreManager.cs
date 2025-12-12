using Core;
using Core.Commands;
using Core.Configs;

namespace WinApp.CoreHandling;

internal class CoreManager
{
    private Core.Controller? _coreController;

    /// <summary>
    /// Create and initialize the core controller.
    /// </summary>
    public void Initialize(ControllerInitParams coreControllerInitParams) =>
        _coreController = new(coreControllerInitParams);

    public void Run() =>
        _coreController?.Run();

    public void Stop() =>
        _coreController?.Stop();

    public void OpenLogsFolder()
    {
        var command = new CoreCommand(CoreCommandType.OpenLogsFolder);
        _coreController?.ExecuteCommand(command);
    }

    public void ShowLog(bool showLog)
    {
        var command = new CoreCommand(CoreCommandType.ShowLog, showLog);
        _coreController?.ExecuteCommand(command);
    }

    public void SetLogToFile(bool logToFile)
    {
        var command = new CoreCommand(CoreCommandType.SetLogToFile, logToFile);
        _coreController?.ExecuteCommand(command);
    }

    public void OpenConfigFile()
    {
        var command = new CoreCommand(CoreCommandType.OpenConfigFile);
        _coreController?.ExecuteCommand(command);
    }

    public void UpdateConfig(ConfigInfo configInfo)
    {
        var command = new CoreCommand(CoreCommandType.UpdateConfig, configInfo);
        _coreController?.ExecuteCommand(command);
    }
}
