using Core.Interfaces;
using Core.Models;
using NLog;

namespace Core.Components;

public class Controller
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IUi _ui;
    private readonly ConfigManager _configManager;
    private readonly WatcherManager _watcherManager;
    private List<Config> _configs = [];

    public Controller(IUi ui)
    {
        _ui = ui;
        _configManager = new ConfigManager();
        _watcherManager = new WatcherManager();
    }



    public void Start()
    {
        var configWatcher = _watcherManager.GetConfigWatcher(OnConfigChanged);
        GetConfigs();

        while (true)
            Thread.Sleep(1000);
    }

    public void OnConfigChanged()
    {
        GetConfigs();
    }



    private void GetConfigs()
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
