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
        if (sender != this)
            return;

        string eventType = e.ChangeType.ToString().ToLower();
        _logger.Info($"Config file {eventType}: {e.FullPath}");

        Callback?.Invoke();
    }

    private void OnRenamed(object sender, FileSystemEventArgs e)
    {
        if (sender != this)
            return;

        if (FileManager.FileNameMatchesFilter(e.FullPath, Filter))
            OnChanged(this, e);
        else
            OnMissing(this, e);
    }

    private void OnMissing(object sender, FileSystemEventArgs e)
    {
        if (sender != this)
            return;

        _logger.Warn($"Config file '{Filter}' not found in {Path}");
        Callback?.Invoke();
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        var exception = e.GetException();
        if (exception != null)
            _logger.Warn(exception);
    }
}
