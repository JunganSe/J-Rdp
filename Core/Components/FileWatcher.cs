using Core.Enums;
using Core.Main;
using NLog;

namespace Core.Components;

internal class FileWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Action<WatcherStatus, string> Callback { get; }

    public FileWatcher(string path, string filter, Action<WatcherStatus, string> callback)
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



    private void OnDetected(object sender, FileSystemEventArgs args)
    {
        _logger.Info($"File detected: {args.FullPath}");
        Callback.Invoke(WatcherStatus.FileFound, args.FullPath);
    }

    private void OnRenamed(object sender, FileSystemEventArgs args)
    {
        if (FileSystemHelper.FileNameMatchesFilter(args.FullPath, Filter))
            OnDetected(this, args);
    }

    private void OnError(object sender, ErrorEventArgs args)
    {
        var exception = args.GetException();
        if (exception != null)
            _logger.Error(exception);
    }
}
