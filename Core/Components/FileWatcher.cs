namespace Core.Components;

internal class FileWatcher : FileSystemWatcher
{
    public Action<string>? Callback { get; set; }
}
