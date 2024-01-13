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
    private readonly string _fileNameFilter;
    private string _currentPath = "";

    public Action<FolderWatcher, string> Callback { get; }
    public WatcherStatus Status { get; private set; }

    public FolderWatcher(string fullPath, string fileNameFilter, Action<FolderWatcher, string> callback)
    {
        Status = WatcherStatus.Unknown;
        _fullPath = fullPath;
        _fileNameFilter = fileNameFilter;
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
        _currentPath = IoPath.Combine(Path, Filter);

        string fullPath = IoPath.Combine(_fullPath, _fileNameFilter).NormalizePath();
        _logger.Info($"Watching for '{_currentPath}' in path '{fullPath}'.");
    }



    private void OnDetected(object sender, FileSystemEventArgs args)
    {
        bool fullPathExists = new DirectoryInfo(_fullPath).Exists;
        if (fullPathExists)
        {
            // TODO: Bevaka filen.
        }
        else
            Initialize();
    }

    private void OnRenamed(object sender, RenamedEventArgs args)
    {
        string found = new DirectoryInfo(args.FullPath).FullName.ToUpper();
        string target = new DirectoryInfo(_currentPath).FullName.ToUpper();
        if (found == target)
            OnDetected(this, args);
        else
            Initialize();
    }

    private void OnError(object sender, ErrorEventArgs args)
    {
        var exception = args.GetException();
        if (exception != null)
            _logger.Error(exception);
    }
}
