using Core.Components;
using NLog;

namespace Core.Main;

internal class WatcherManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
            watcher.Created += FileWatcher.OnDetected;
            watcher.Renamed += FileWatcher.OnRenamed;
            watcher.Error += OnError;
            return watcher;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create file watcher.");
            return null;
        }
    }



    private void OnError(object sender, ErrorEventArgs e)
    {
        var exception = e.GetException();
        if (exception == null)
            return;
        _logger.Warn(exception);
    }
}
