namespace Core.Components;

public class FileWatcher : FileSystemWatcher
{
    private readonly Action _callback;

    public FileWatcher(Action callback)
    {
        _callback = callback;
    }

    public void InvokeCallback() => _callback.Invoke();
}
