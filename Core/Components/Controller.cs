using Core.Interfaces;
using Core.Models;
using NLog;

namespace Core.Components;

public class Controller
{
    private const int DEBOUNCE_MS = 500;

    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IUi _ui;
    private readonly ConfigManager _configManager;
    private readonly WatcherManager _watcherManager;
    private List<Config> _configs = [];
    private DateTime _lastConfigChange;

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
        if (DebounceConfig())
        {
            _logger.Trace("Debounced config change.");
            return;
        }

        GetConfigs();
    }



    private void GetConfigs()
    {
        _configs = _configManager.GetConfigs();
        _lastConfigChange = DateTime.Now;
        _logger.Info("Configs updated.");
    }

    private bool DebounceConfig() // True if the last config change was within {DEBOUNCE_MS} milliseconds. 
    {
        var time = DateTime.Now - _lastConfigChange;
        Console.WriteLine($"{DateTime.Now} - {time}");
        return (time.TotalMilliseconds < DEBOUNCE_MS);
    }
}
