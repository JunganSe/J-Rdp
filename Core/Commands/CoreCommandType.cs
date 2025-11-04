namespace Core.Commands;

public enum CoreCommandType
{
    Initialize,
    Run,
    Stop,
    SetLogConsoleVisibility,
    SetLogToFile,
    OpenLogsFolder,
    OpenConfigFile,
    UpdateConfig,
    SetCallback_ConfigUpdated,
}
