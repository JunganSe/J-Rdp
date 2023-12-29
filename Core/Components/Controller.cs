using Core.Interfaces;
using Core.Models;

namespace Core.Components;

public class Controller
{
    private const int DEBOUNCE_MS = 500;

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
        _configs = _configManager.GetConfigs();
        _lastConfigChange = DateTime.Now;
        var configWatcher = _watcherManager.GetConfigWatcher(OnConfigChanged);

        while (true)
            Thread.Sleep(1000);
    }

    public void OnConfigChanged()
    {
        if (DebounceConfig())
            return;

        _configs = _configManager.GetConfigs();
        _lastConfigChange = DateTime.Now;
    }



    private bool DebounceConfig() // True if the last config change was within {DEBOUNCE_MS} milliseconds. 
    {
        var time = DateTime.Now - _lastConfigChange;
        Console.WriteLine($"{DateTime.Now} - {time}");
        return (time.TotalMilliseconds < DEBOUNCE_MS);
    }
}
