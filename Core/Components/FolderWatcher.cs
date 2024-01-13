using Core.Main;
using NLog;

namespace Core.Components;

internal class FolderWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _currentTargetPath;
    private readonly string _fullPath;
    private readonly string? _fileNameFilter;
    private readonly FileWatcher? _governingFileWatcher;

    public FolderWatcher(string fullPath, string fileNameFilter)
    {
        Initialize(fullPath);
        _currentTargetPath = System.IO.Path.Combine(Path, Filter);
        _fullPath = fullPath;
        _fileNameFilter = fileNameFilter;
        string fullTargetPath = System.IO.Path.Combine(fullPath, fileNameFilter);
        _logger.Info($"Watching for '{_currentTargetPath}' in path '{fullTargetPath}'.");
    }

    public FolderWatcher(string fullPath, FileWatcher fileWatcher)
    {
        Initialize(fullPath);
        _currentTargetPath = System.IO.Path.Combine(Path, Filter);
        _fullPath = fullPath;
        _governingFileWatcher = fileWatcher;
        string fullTargetPath = System.IO.Path.Combine(fileWatcher.Path, fileWatcher.Filter);
        _logger.Info($"Watching for '{_currentTargetPath}' in path '{fullTargetPath}'.");
    }

    private void Initialize(string fullPath)
    {
        Path = FileSystemHelper.GetLastExistingFolderPath(fullPath);
        Filter = FileSystemHelper.GetFirstMissingFolderName(fullPath) ?? "";
        EnableRaisingEvents = true;
        IncludeSubdirectories = false;
        NotifyFilter = NotifyFilters.DirectoryName;
        Created += OnDetected;
        Renamed += OnRenamed;
        Deleted += OnDeleted;
        Error += OnError;
    }



    private void OnDetected(object sender, FileSystemEventArgs args)
    {
        if (true) // Om sista mappen finns
        {
            // TODO: Bevaka filen.
        }
        else
        {
            // TODO: Bevaka nästa mapp och stäng av denna bevakning. eller ändra?
        }
    }

    private void OnRenamed(object sender, RenamedEventArgs args)
    {
        string found = new DirectoryInfo(args.FullPath).FullName.ToUpper();
        string target = new DirectoryInfo(_currentTargetPath).FullName.ToUpper();
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
