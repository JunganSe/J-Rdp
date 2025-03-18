using Core.Constants;
using Core.Helpers;
using NLog;

namespace Core.Workers;

internal class ConfigWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly Action _callback;
    private Timer? _debounceTimer;

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

        // Debounce to prevent multiple events from firing during the same updating of the file.
        // Only run the handler method if the event hasn't been fired in x ms.
        _debounceTimer?.Dispose();
        _debounceTimer = new Timer(_ => HandleConfigFileChanged(args), null,
            FileConstants.FileChanged_DebounceDelay, Timeout.Infinite);
    }

    private void HandleConfigFileChanged(FileSystemEventArgs args)
    {
        string eventType = args.ChangeType.ToString().ToLower();
        _logger.Info($"Config file {eventType}.");

        _callback.Invoke();

        _debounceTimer?.Dispose();
        _debounceTimer = null;
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
        _callback.Invoke();
    }

    private void OnError(object sender, ErrorEventArgs args)
    {
        var exception = args.GetException();
        if (exception != null)
            _logger.Error(exception);
    }
}
