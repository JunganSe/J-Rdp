using Core.Interfaces;
using Core.Models;

namespace Core.Components;

public class Controller
{
    private readonly IUi _ui;
    private readonly ConfigManager _configManager;
    private readonly WatcherManager _watcherManager;
    private List<Config> _configs = [];

    public Controller(IUi ui)
    {
        _ui = ui;
        _configManager = ConfigManager.Instance;
        _watcherManager = new WatcherManager();
    }



    public void Start()
    {
        _configs = _configManager.GetConfigs();
        var configWatcher = _watcherManager.GetConfigWatcher(OnConfigChanged);

        while (true)
            Thread.Sleep(1000);
    }

    public void OnConfigChanged()
    {
        // TODO: Debounce: Avbryt om samma fil har ändrats inom 500ms.
        _configs = _configManager.GetConfigs();
    }
}
