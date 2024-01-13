using Core.Enums;
using Core.Extensions;
using Core.Main;
using NLog;
using IoPath = System.IO.Path;

namespace Core.Components;

internal class FileWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Action<FileWatcher, WatcherStatus, string> Callback { get; }

    public FileWatcher(string path, string filter, Action<FileWatcher, WatcherStatus, string> callback)
    {
        Path = path;
        Filter = filter;
        Callback = callback;
        EnableRaisingEvents = true;
        IncludeSubdirectories = false;
        NotifyFilter = NotifyFilters.FileName;
        Created += OnDetected;
        Renamed += OnRenamed;
        Error += OnError;
    }



    private void OnDetected(object sender, FileSystemEventArgs args)
    {
        _logger.Info($"File detected: {args.FullPath}");
        Callback.Invoke(this, WatcherStatus.FileFound, args.FullPath);
    }

    private void OnRenamed(object sender, FileSystemEventArgs args)
    {
        if (FileSystemHelper.FileNameMatchesFilter(args.FullPath, Filter))
            OnDetected(this, args);
    }

    private void OnError(object sender, ErrorEventArgs args)
    {
        var status = WatcherStatus.UnknownError;
        string message = $"Error when watching for '{Filter}' in: {Path}";

        bool pathExists = new DirectoryInfo(Path).Exists;
        if (!pathExists)
        {
            status = WatcherStatus.WatchFolderMissing;
            message = $"The watched directory is missing: {Path}";
        }

        var exception = args.GetException() ?? new Exception("Unknown error.");
        _logger.Warn(exception, message);

        string fullPath = IoPath.Combine(Path, Filter).NormalizePath();
        Callback.Invoke(this, status, fullPath);
    }
}
