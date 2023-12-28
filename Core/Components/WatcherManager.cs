using NLog;

namespace Core.Components;

public class WatcherManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly Controller _controller;

    public WatcherManager(Controller controller)
    {
        _controller = controller;
    }



    public FileSystemWatcher GetConfigWatcher()
    {
        var watcher = new FileSystemWatcher();
        watcher.Path = AppDomain.CurrentDomain.BaseDirectory;
        watcher.Filter = "config.json";
        watcher.EnableRaisingEvents = true;
        watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime;
        watcher.Created += OnConfigChanged;
        watcher.Changed += OnConfigChanged;
        watcher.Deleted += OnConfigChanged;
        watcher.Error += OnError;
        return watcher;
    }



    private void OnConfigChanged(object sender, FileSystemEventArgs e)
    {
        string type = e.ChangeType.ToString().ToLower();
        _logger.Info($"Config file {type}: {e.FullPath}");
        _controller.OnConfigChanged();
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        var exception = e.GetException();
        if (exception == null)
            return;
        _logger.Warn(exception);
    }
}
