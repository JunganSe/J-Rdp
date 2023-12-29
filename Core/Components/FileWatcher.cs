namespace Core.Components;

public class FileWatcher : FileSystemWatcher
{
    public Action<string>? Callback { get; set; }
}