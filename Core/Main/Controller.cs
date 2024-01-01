using Core.Interfaces;
using Core.Components;
using NLog;

namespace Core.Main;

public class Controller
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IUi _ui;
    private readonly string _configDirectory;
    private readonly string _configFileName;
    private readonly ConfigManager _configManager;
    private readonly WatcherManager _watcherManager;
    private List<Config> _configs = [];
    private List<FileWatcher> _fileWatchers = [];

    public Controller(IUi ui)
    {
        _ui = ui;
        _configDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _configFileName = "config.json";
        _configManager = new ConfigManager(_configDirectory + _configFileName);
        _watcherManager = new WatcherManager();
    }



    public void Start()
    {
        var configWatcher = _watcherManager.GetConfigWatcher(_configDirectory, _configFileName, OnConfigChanged);
        _logger.Debug($"Watching for '{_configFileName}' at {_configDirectory}");
        UpdateConfigs();
        SetFileWatchers();
        LogWatchers();

        while (true)
            Thread.Sleep(1000);
    }

    internal void OnConfigChanged()
    {
        UpdateConfigs();
        SetFileWatchers();
        LogWatchers();
    }

    internal void OnFileDetected(string fullPath)
    {
        throw new NotImplementedException();
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
        {
            var watcher = _fileWatchers[i];
            _fileWatchers.Remove(watcher);
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
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

        string filter = config.Filter;
        var watcher = _watcherManager.GetFileWatcher(folder, filter, OnFileDetected);

        if (watcher is null)
            return false;

        _fileWatchers.Add(watcher);
        return true;
    }

    private void LogWatchers()
    {
        string filterList = (_configs.Any())
            ? string.Join("", _configs.Select(c => $"\n  '{c.Filter}' in {c.WatchFolder}"))
            : "(nothing)";
        _logger.Info($"Currently watching for: {filterList}");
    }
}
