using NLog;

namespace Core.Components;

internal class FileWatcher : FileSystemWatcher
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Action<string>? Callback { get; set; }

    public static void OnDetected(object sender, FileSystemEventArgs e)
    {
        if (sender is not FileWatcher watcher)
            return;

        _logger.Info($"File detected: {e.FullPath}");
        watcher.Callback?.Invoke(e.FullPath);
    }

    public static void OnRenamed(object sender, FileSystemEventArgs e)
    {
        // TODO: Kontrollera att filen fortfarande matchar filtret.

        OnDetected(sender, e);
    }
}
