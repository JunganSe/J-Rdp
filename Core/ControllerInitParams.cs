using Core.Configs;
using Core.LogDisplay;

namespace Core;

public record ControllerInitParams(
    Handler_OnConfigUpdated Callback_ConfigUpdated,
    ILogDisplayManager LogDisplayManager,
    Action Callback_LogClosed);
