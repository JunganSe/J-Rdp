using NLog;

namespace Core.Components;

internal class FileWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Action<string>? Callback { get; set; }

    public void OnDetected(object sender, FileSystemEventArgs e)
    {
        if (sender is not FileWatcher watcher)
            return;

        _logger.Info($"New file detected: {e.FullPath}");
        watcher.Callback?.Invoke(e.FullPath);
    }

    public void OnRenamed(object sender, FileSystemEventArgs e)
    {
        // TODO: Kontrollera att filen fortfarande matchar filtret.

        OnDetected(sender, e);
    }
}
