﻿using Core.Main;
using NLog;

namespace Core.Components;

internal class FolderWatcher : FileSystemWatcher
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _fullTargetPath;

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

    private void OnRenamed(object sender, FileSystemEventArgs args)
    {
        // TODO: Kontrollera att det stämmer.
        OnDetected(this, args);
    }

    private void OnError(object sender, ErrorEventArgs args)
    {
        var exception = args.GetException();
        if (exception != null)
            _logger.Error(exception);
    }
}