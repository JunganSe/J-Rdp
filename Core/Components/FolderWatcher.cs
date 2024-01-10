using Core.Main;
using NLog;

namespace Core.Components;

internal class FolderWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _fullTargetPath;
    private readonly string _currentTargetPath;

    public FolderWatcher(string path, string fileNameFilter)
    {
        _fullTargetPath = System.IO.Path.Combine(path, fileNameFilter);

        Path = FileManager.GetLastExistingFolderName(path);
        Filter = FileManager.GetFirstMissingFolderName(path);
        EnableRaisingEvents = true;
        IncludeSubdirectories = false;
        NotifyFilter = NotifyFilters.DirectoryName;
        Created += OnDetected;
        Renamed += OnRenamed;
        Error += OnError;

        _currentTargetPath = System.IO.Path.Combine(Path, Filter);
        _logger.Info($"Watching for folder '{Filter}' in: {Path}");
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
    }

    private void OnError(object sender, ErrorEventArgs args)
    {
        var exception = args.GetException();
        if (exception != null)
            _logger.Error(exception);
    }
}
