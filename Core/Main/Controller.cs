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
        UpdateConfigs();

        while (true)
            Thread.Sleep(1000);
    }

    public void OnConfigChanged()
    {
        UpdateConfigs();
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
}
