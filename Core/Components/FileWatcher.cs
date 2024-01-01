using Core.Main;
using NLog;

namespace Core.Components;

internal class FileWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Action<string>? Callback { get; set; }

    public FileWatcher(string path, string filter, Action<string> callback)
    {
        Path = path;
        Filter = filter;
        Callback = callback;
        EnableRaisingEvents = true;
        IncludeSubdirectories = false;
        NotifyFilter = NotifyFilters.FileName;
        Created += OnDetected;
        Renamed += OnRenamed;
        Error += OnError;
    }



    private void OnDetected(object sender, FileSystemEventArgs e)
    {
        if (sender != this)
            return;

        _logger.Info($"File detected: {e.FullPath}");
        Callback?.Invoke(e.FullPath);
    }

    private void OnRenamed(object sender, FileSystemEventArgs e)
    {
        if (sender != this)
            return;

        if (FileManager.FileNameMatchesFilter(e.FullPath, Filter))
            OnDetected(this, e);
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        var exception = e.GetException();
        if (exception != null)
            _logger.Warn(exception);
    }
}
