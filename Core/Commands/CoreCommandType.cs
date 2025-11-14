namespace Core.Commands;

public enum CoreCommandType
{
    ShowLogDisplay,
    SetLogToFile,
    OpenLogsFolder,
    OpenConfigFile,
    UpdateConfig,
    SetCallback_ConfigUpdated,
    SetLogDisplayManager,
    SetCallback_LogClosed,
}
