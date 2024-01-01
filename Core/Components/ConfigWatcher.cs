using Core.Main;
using NLog;

namespace Core.Components;

internal class ConfigWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Action? Callback { get; set; }

    public ConfigWatcher(string path, string filter, Action callback)
    {
        Path = path;
        Filter = filter;
        Callback = callback;
        EnableRaisingEvents = true;
        IncludeSubdirectories = false;
        NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
        Created += OnChanged;
        Changed += OnChanged;
        Renamed += OnRenamed;
        Deleted += OnMissing;
        Error += OnError;
    }



    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (sender is not ConfigWatcher watcher)
            return;

        string eventType = e.ChangeType.ToString().ToLower();
        _logger.Info($"Config file {eventType}: {e.FullPath}");

        watcher.Callback?.Invoke();
    }

    private void OnRenamed(object sender, FileSystemEventArgs e)
    {
        if (sender is not ConfigWatcher watcher)
            return;

        if (FileManager.FileNameMatchesFilter(e.FullPath, watcher.Filter))
            OnChanged(watcher, e);
        else
            OnMissing(watcher, e);
    }

    private void OnMissing(object sender, FileSystemEventArgs e)
    {
        if (sender is not ConfigWatcher watcher)
            return;

        var folderPath = System.IO.Path.GetDirectoryName(e.FullPath);
        var fileName = System.IO.Path.GetFileName(e.FullPath);
        _logger.Warn($"Config file '{fileName}' not found in: {folderPath}");

        watcher.Callback?.Invoke();
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        var exception = e.GetException();
        if (exception == null)
            return;
        _logger.Warn(exception);
    }
}
