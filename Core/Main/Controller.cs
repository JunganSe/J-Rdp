using Core.Constants;
using Core.Managers;
using Core.Workers;
using NLog;

namespace Core.Main;

public class Controller
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly ConfigWatcherWorker _configWatcherWorker = new();
    private readonly ConfigWorker _configWorker = new();
    private readonly ProfileManager _profilemanager = new();
    private readonly FileManager _fileManager = new();
    private int _pollingInterval = ConfigConstants.PollingInterval_Default;

    public void Run()
    {
        try
        {
            Initialize();

            while (true)
            {
                MainLoop();
                Thread.Sleep(_pollingInterval);
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "Unhandled exception.");
            return;
        }
    }

    private void Initialize()
    {
        _configWatcherWorker.StopAndDisposeConfigWatcher();
        _configWatcherWorker.StartConfigWatcher(callback: InitializeConfig);
        InitializeConfig();
    }

    private void InitializeConfig()
    {
        _configWorker.UpdateConfig();
        SetPollingInterval();
        _fileManager.SetDeleteDelay(_configWorker.GetDeleteDelay());
        InitializeProfiles();
    }

    private void SetPollingInterval()
    {
        int newPollingInterval = _configWorker.GetPollingInterval();
        if (newPollingInterval == _pollingInterval)
            return;

        _pollingInterval = newPollingInterval;
        _logger.Info($"Polling interval set to {_pollingInterval} ms.");
    }

    private void InitializeProfiles()
    {
        _profilemanager.UpdateProfileInfos(_configWorker.Profiles);
        _profilemanager.UpdateProfileInfosFiles();
        _profilemanager.LogProfileInfosSummary();
    }

    private void MainLoop()
    {
        _profilemanager.UpdateProfileInfosFiles();
        _fileManager.ProcessProfileInfos(_profilemanager.ProfileInfos);
    }
}
