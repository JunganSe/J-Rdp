namespace Core.Models;

public class ConfigWatcher : FileSystemWatcher
{
    public Action? Callback { get; set; }
}
