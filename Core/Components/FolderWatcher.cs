using Core.Main;
using NLog;

namespace Core.Components;

internal class FolderWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _fullPath;
    private readonly string? _fileNameFilter;
    private readonly FileWatcher? _governingFileWatcher;
    private string _currentPath = "";

    public FolderWatcher(string fullPath, string fileNameFilter)
    {
        _fullPath = fullPath;
        _fileNameFilter = fileNameFilter;
        Initialize();
    }

    public FolderWatcher(string fullPath, FileWatcher fileWatcher)
    {
        _fullPath = fullPath;
        _governingFileWatcher = fileWatcher;
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
        Deleted += OnDeleted;
        Error += OnError;
        _currentPath = System.IO.Path.Combine(Path, Filter);

        string fullPath = (_governingFileWatcher != null) 
            ? FileSystemHelper.CombineAndNormalizePaths(_governingFileWatcher.Path, _governingFileWatcher.Filter) 
            : FileSystemHelper.CombineAndNormalizePaths(_fullPath, _fileNameFilter ?? "");
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
        {
            // TODO: Hantera att mappen inte finns längre.
        }
    }

    private void OnDeleted(object sender, FileSystemEventArgs args)
    {
        // TODO: Hantera att mappen inte finns längre.
        throw new NotImplementedException();
    }

    private void OnError(object sender, ErrorEventArgs args)
    {
        var exception = args.GetException();
        if (exception != null)
            _logger.Error(exception);
    }
}
