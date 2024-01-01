using Core.Main;
using NLog;

namespace Core.Components;

internal class ConfigWatcher : FileSystemWatcher
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Action? Callback { get; set; }

    public static void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (sender is not ConfigWatcher watcher)
            return;

        string eventType = e.ChangeType.ToString().ToLower();
        _logger.Info($"Config file {eventType}: {e.FullPath}");

        watcher.Callback?.Invoke();
    }

    public static void OnRenamed(object sender, FileSystemEventArgs e)
    {
        if (sender is not ConfigWatcher watcher)
            return;

        if (FileManager.FileNameMatchesFilter(e.FullPath, watcher.Filter))
            OnChanged(watcher, e);
    }
}
