using Core.Components;
using Core.Enums;
using Core.Extensions;
using NLog;

namespace Core.Main;

public class Controller
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _configDirectory;
    private readonly string _configFileName;
    private readonly ConfigManager _configManager;
    private readonly List<FileWatcher> _fileWatchers = [];
    private List<Config> _configs = [];

    public Controller()
    {
        _configDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _configFileName = "config.json";
        _configManager = new ConfigManager(_configDirectory + _configFileName);
    }



    public void Start()
    {
        var configWatcher = new ConfigWatcher(_configDirectory, _configFileName, OnConfigChanged);
        _logger.Debug($"Watching for config file '{_configFileName}' in {_configDirectory}");
        UpdateConfigs();
        SetFileWatchers();
        LogConfigSummary();

        while (true)
            Thread.Sleep(1000);
    }

    internal void OnConfigChanged()
    {
        UpdateConfigs();
        SetFileWatchers();
        LogConfigSummary();
    }

    internal void FileWatcherCallback(FileWatcher fileWatcher, string fullPath)
    {
        switch (fileWatcher.Status)
        {
            case WatcherStatus.FileFound:
                // TODO: Hantera filen.
                break;

            case WatcherStatus.WatchFolderMissing:
                // TODO: Set up folder watcher.
                break;

            case WatcherStatus.UnknownError:
                RemoveFileWatcher(fileWatcher);
                break;
        }
    }



    private void UpdateConfigs()
    {
        var configs = _configManager.GetConfigs();
        if (configs is null)
        {
            _logger.Warn("Failed to update configs.");
            _configs.Clear();
            return;
        }

        _configs = configs;
        _logger.Info("Configs updated.");
    }

    private void ClearFileWatchers()
    {
        for (int i = _fileWatchers.Count - 1; i >= 0; i--)
            RemoveFileWatcher(_fileWatchers[i]);
    }

    private void RemoveFileWatcher(FileWatcher fileWatcher)
    {
        _fileWatchers.Remove(fileWatcher);
        fileWatcher.EnableRaisingEvents = false;
        fileWatcher.Dispose();
    }

    private void SetFileWatchers()
    {
        ClearFileWatchers();
        foreach (var config in _configs)
        {
            bool success = TryAddFileWatcher(config);
            if (!success)
            {
                // TODO: Lägg bevakning på parent-mappen och skapa filewatcher om mappen dyker upp.
                var folderWatcher = new FolderWatcher(config.WatchFolder, config.Filter);
            }
        }
    }

    private bool TryAddFileWatcher(Config config)
    {
        string folder = config.WatchFolder;
        if (!Directory.Exists(folder))
        {
            _logger.Warn($"Config '{config.Name}' tried to watch folder that does not exist: {folder}");
            return false;
        }

        var watcher = new FileWatcher(folder, config.Filter, FileWatcherCallback);
        _fileWatchers.Add(watcher);
        return true;
    }

    private void LogConfigSummary()
    {
        string configsSummary = (_configs.Count > 0)
            ? string.Join("", _configs.Select(c => $"\n  {c.Name}: '{c.Filter}' in: {c.WatchFolder.NormalizePath()}"))
            : "(nothing)";
        _logger.Info($"Current configs: {configsSummary}");
    }
}
