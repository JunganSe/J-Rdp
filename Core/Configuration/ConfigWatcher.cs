using Core.Helpers;
using NLog;

namespace Core.Configuration;

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



    private void OnChanged(object sender, FileSystemEventArgs args)
    {
        if (sender != this)
            return;

        string eventType = args.ChangeType.ToString().ToLower();
        _logger.Info($"Config file {eventType}: {args.FullPath}");

        Callback?.Invoke();
    }

    private void OnRenamed(object sender, FileSystemEventArgs args)
    {
        if (sender != this)
            return;

        if (FileSystemHelper.FileNameMatchesFilter(args.FullPath, Filter))
            OnChanged(this, args);
        else
            OnMissing(this, args);
    }

    private void OnMissing(object sender, FileSystemEventArgs args)
    {
        if (sender != this)
            return;

        _logger.Warn($"Config file '{Filter}' not found in {Path}");
        Callback?.Invoke();
    }

    private void OnError(object sender, ErrorEventArgs args)
    {
        var exception = args.GetException();
        if (exception != null)
            _logger.Error(exception);
    }
}
