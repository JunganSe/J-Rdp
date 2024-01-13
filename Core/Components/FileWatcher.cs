using Core.Enums;
using Core.Extensions;
using Core.Main;
using NLog;
using IoPath = System.IO.Path;

namespace Core.Components;

internal class FileWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Action<FileWatcher, string> Callback { get; }
    public WatcherStatus Status { get; private set; }

    public FileWatcher(string path, string filter, Action<FileWatcher, string> callback)
    {
        Status = WatcherStatus.Unknown;
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
        // TODO: Confirm that the path is still the same.
        // Detection can occur even when the folder has been renamed.
        // Problem: args.FullPath still provides the old path from before the folder was renamed, not the new path.

        if (!File.Exists(args.FullPath))
            return;

        _logger.Info($"File detected: {args.FullPath}");
        Status = WatcherStatus.FileFound;
        Callback.Invoke(this, args.FullPath);
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

        if (!Directory.Exists(Path))
        {
            status = WatcherStatus.WatchFolderMissing;
            message = $"The watched directory is missing: {Path}";
        }

        var exception = args.GetException() ?? new Exception("Unknown error.");
        _logger.Warn(exception, message);

        string fullPath = IoPath.Combine(Path, Filter).NormalizePath();
        Status = status;
        Callback.Invoke(this, fullPath);
    }
}
