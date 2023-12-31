using NLog;

namespace Core.Components;

internal class ConfigWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Action? Callback { get; set; }

    public void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (sender is not ConfigWatcher watcher)
            return;

        string eventType = e.ChangeType.ToString().ToLower();
        _logger.Info($"Config file {eventType}: {e.FullPath}");

        watcher.Callback?.Invoke();
    }

    public void OnRenamed(object sender, FileSystemEventArgs e)
    {
        // TODO: Kontrollera att filen fortfarande matchar filtret.

        OnChanged(sender, e);
    }
}
