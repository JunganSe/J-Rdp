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

        while (true)
            Thread.Sleep(1000);
    }

    internal void OnConfigChanged()
    {
        UpdateConfigs();
        SetFileWatchers();
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
            _logger.Warn("Failed to retrieve configs");
            return;
        }

        _configs = configs;
        _logger.Info("Configs updated.");
    }

    private void ClearFileWatchers()
    {
        // TODO: Avsluta och ta bort befintliga watchers.
    }

    private void SetFileWatchers()
    {
        ClearFileWatchers();
        foreach (var config in _configs)
        {
            // TODO: Skapa mapp config.WatchFolder om den inte finns. Gör det med en annan klass.
            string folder = config.WatchFolder;
            string fileNameFilter = config.Filter;
            var watcher = _watcherManager.GetFileWatcher(folder, fileNameFilter, OnFileDetected);
            if (watcher != null)
            {
                _fileWatchers.Add(watcher);
                _logger.Info($"Watching for '{fileNameFilter}' at {folder}");
            }
        }
    }
}
