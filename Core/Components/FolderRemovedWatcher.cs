using NLog;

namespace Core.Components;

internal class FolderRemovedWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _fullPath;
    private readonly FileWatcher _fileWatcher;

    public FolderRemovedWatcher(string fullPath, FileWatcher fileWatcher)
    {
        _fullPath = fullPath;
        _fileWatcher = fileWatcher;

        var dir = new DirectoryInfo(fullPath);

        Path = dir.Parent?.FullName ?? dir.Root.FullName;
        Filter = dir.Name;
        EnableRaisingEvents = true;
        IncludeSubdirectories = false;
        NotifyFilter = NotifyFilters.DirectoryName;
        Renamed += OnRenamed;
        Deleted += OnDeleted;
        Error += OnError;
    }

    private void OnRenamed(object sender, RenamedEventArgs args)
    {
        
    }

    private void OnDeleted(object sender, FileSystemEventArgs args)
    {
        
    }

    private void OnError(object sender, ErrorEventArgs args)
    {
        var exception = args.GetException();
        if (exception != null)
            _logger.Error(exception);
    }
}
