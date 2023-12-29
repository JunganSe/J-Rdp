namespace Core.Models;

public class FileWatcher : FileSystemWatcher
{
    public Action<string>? Callback { get; set; }
}