using Core.Components;
using NLog;

namespace Core.Main;

internal class WatcherManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public ConfigWatcher? GetConfigWatcher(string path, string filter, Action callback)
    {
        try
        {
            var watcher = new ConfigWatcher();
            watcher.Path = path;
            watcher.Filter = filter;
            watcher.Callback = callback;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = false;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            watcher.Created += OnConfigChanged;
            watcher.Changed += OnConfigChanged;
            watcher.Error += OnError;
            return watcher;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create config watcher.");
            return null;
        }

    }

    public FileWatcher? GetFileWatcher(string path, string filter, Action<string> callback)
    {
        try
        {
            var watcher = new FileWatcher();
            watcher.Path = path;
            watcher.Filter = filter;
            watcher.Callback = callback;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = false;
            watcher.NotifyFilter = NotifyFilters.FileName;
            watcher.Created += OnFileDetected;
            watcher.Renamed += OnFileDetected;
            watcher.Error += OnError;
            return watcher;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create file watcher.");
            return null;
        }
    }



    private void OnConfigChanged(object sender, FileSystemEventArgs e)
    {
        if (sender is not ConfigWatcher fileWatcher)
            return;

        string eventType = e.ChangeType.ToString().ToLower();
        _logger.Info($"Config file {eventType}: {e.FullPath}");

        fileWatcher.Callback?.Invoke();
    }

    private void OnFileDetected(object sender, FileSystemEventArgs e)
    {
        if (sender is not FileWatcher fileWatcher)
            return;

        _logger.Info($"New file detected: {e.FullPath}");
        fileWatcher.Callback?.Invoke(e.FullPath);
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        var exception = e.GetException();
        if (exception == null)
            return;
        _logger.Warn(exception);
    }
}
