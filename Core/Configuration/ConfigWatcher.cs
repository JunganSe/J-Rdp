using Core.Helpers;
using NLog;

namespace Core.Configuration;

internal class ConfigWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly Action? _callback;

    public ConfigWatcher(string path, string filter, Action callback)
    {
        _logger.Trace("Starting...");

        Path = path;
        Filter = filter;
        _callback = callback;
        EnableRaisingEvents = true;
        IncludeSubdirectories = false;
        NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
        Created += OnChanged;
        Changed += OnChanged;
        Renamed += OnRenamed;
        Deleted += OnMissing;
        Error += OnError;

        _logger.Debug($"Watching for '{filter}' in '{path}'.");
    }



    private void OnChanged(object sender, FileSystemEventArgs args)
    {
        if (sender != this)
            return;

        string eventType = args.ChangeType.ToString().ToLower();
        _logger.Info($"Config file {eventType}.");

        _callback?.Invoke();
    }

    private void OnRenamed(object sender, FileSystemEventArgs args)
    {
        if (sender != this)
            return;

        if (FileHelper.FileNameMatchesFilter(args.FullPath, Filter))
            OnChanged(this, args);
        else
            OnMissing(this, args);
    }

    private void OnMissing(object sender, FileSystemEventArgs args)
    {
        if (sender != this)
            return;

        _logger.Warn($"Config file '{Filter}' not found in '{Path}'.");
        _callback?.Invoke();
    }

    private void OnError(object sender, ErrorEventArgs args)
    {
        var exception = args.GetException();
        if (exception != null)
            _logger.Error(exception);
    }
}
