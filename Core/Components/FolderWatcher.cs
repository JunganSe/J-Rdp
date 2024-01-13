using Core.Enums;
using Core.Extensions;
using Core.Main;
using NLog;
using IoPath = System.IO.Path;

namespace Core.Components;

internal class FolderWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _fullPath;
    private string _currentPath = "";

    public Action<FolderWatcher, string> Callback { get; }
    public WatcherStatus Status { get; private set; }

    public FolderWatcher(string fullPath, Action<FolderWatcher, string> callback)
    {
        Status = WatcherStatus.Unknown;
        _fullPath = fullPath.NormalizePath();
        Callback = callback;
        Initialize();
    }

    private void Initialize()
    {
        Path = FileSystemHelper.GetLastExistingFolderPath(_fullPath);
        Filter = FileSystemHelper.GetFirstMissingFolderName(_fullPath) ?? "";
        EnableRaisingEvents = true;
        IncludeSubdirectories = false;
        NotifyFilter = NotifyFilters.DirectoryName;
        Created += OnDetected;
        Renamed += OnRenamed;
        Error += OnError;
        _currentPath = IoPath.Combine(Path, Filter).NormalizePath();
        _logger.Info($"Watching for '{_currentPath}' in path '{_fullPath}'.");
    }



    private void OnDetected(object sender, FileSystemEventArgs args)
    {
        bool fullFolderPathExists = Directory.Exists(_fullPath);
        if (fullFolderPathExists)
        {
            Status = WatcherStatus.FolderFound;
            Callback.Invoke(this, _fullPath);
        }
        else
            Initialize();
    }

    private void OnRenamed(object sender, RenamedEventArgs args)
    {
        string found = args.FullPath.NormalizePath().ToUpper();
        string target = _currentPath.NormalizePath().ToUpper();
        bool targetExists = Directory.Exists(target);
        if (found == target && targetExists)
            OnDetected(this, args);
    }

    private void OnError(object sender, ErrorEventArgs args)
    {
        var exception = args.GetException();
        if (exception != null)
            _logger.Error(exception);
    }
}
