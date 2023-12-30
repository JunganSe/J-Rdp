namespace Core.Components;

internal class ConfigWatcher : FileSystemWatcher
{
    public Action? Callback { get; set; }
}
