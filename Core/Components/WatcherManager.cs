using NLog;

namespace Core.Components;

public class WatcherManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public FileSystemWatcher GetConfigWatcher(Action callback)
    {
        var watcher = new FileWatcher();
        watcher.Path = AppDomain.CurrentDomain.BaseDirectory;
        watcher.Filter = "config.json";
        watcher.Callback = callback;
        watcher.EnableRaisingEvents = true;
        watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
        watcher.Created += OnConfigChanged;
        watcher.Changed += OnConfigChanged;
        watcher.Deleted += OnConfigChanged;
        watcher.Error += OnError;
        return watcher;
    }



    private void OnConfigChanged(object sender, FileSystemEventArgs e)
    {
        if (sender is not FileWatcher fileWatcher)
            return;

        string type = e.ChangeType.ToString().ToLower();
        _logger.Info($"Config file {type}: {e.FullPath}");

        bool isFileDeleted = e.ChangeType.HasFlag(WatcherChangeTypes.Deleted);
        if (!isFileDeleted)
            fileWatcher.Callback?.Invoke();
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        var exception = e.GetException();
        if (exception == null)
            return;
        _logger.Warn(exception);
    }
}
