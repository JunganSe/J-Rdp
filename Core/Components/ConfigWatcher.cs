namespace Core.Components;

public class ConfigWatcher : FileSystemWatcher
{
    public Action? Callback { get; set; }
}
