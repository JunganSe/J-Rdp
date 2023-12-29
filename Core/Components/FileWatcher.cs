namespace Core.Components;

public class FileWatcher : FileSystemWatcher
{
    public Action? Callback { get; set; }
}
