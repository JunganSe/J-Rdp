using Core.Enums;
using Core.Extensions;
using Core.Main;
using NLog;
using IoPath = System.IO.Path;

namespace Core.Components;

internal class FolderWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _fullFolderPath;
    private readonly string _fileNameFilter;
    private string _currentPath = "";

    public Action<FolderWatcher, string> Callback { get; }
    public WatcherStatus Status { get; private set; }

    public FolderWatcher(string fullFolderPath, string fileNameFilter, Action<FolderWatcher, string> callback)
    {
        Status = WatcherStatus.Unknown;
        _fullFolderPath = fullFolderPath;
        _fileNameFilter = fileNameFilter;
        Callback = callback;
        Initialize();
    }

    private void Initialize()
    {
        Path = FileSystemHelper.GetLastExistingFolderPath(_fullFolderPath);
        Filter = FileSystemHelper.GetFirstMissingFolderName(_fullFolderPath) ?? "";
        EnableRaisingEvents = true;
        IncludeSubdirectories = false;
        NotifyFilter = NotifyFilters.DirectoryName;
        Created += OnDetected;
        Renamed += OnRenamed;
        Error += OnError;
        _currentPath = IoPath.Combine(Path, Filter).NormalizePath();

        string fullPath = IoPath.Combine(_fullFolderPath, _fileNameFilter).NormalizePath();
        _logger.Info($"Watching for '{_currentPath}' in path '{fullPath}'.");
    }



    private void OnDetected(object sender, FileSystemEventArgs args)
    {
        bool fullFolderPathExists = Directory.Exists(_fullFolderPath);
        if (fullFolderPathExists)
        {
            // TODO: Bevaka filen.
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
