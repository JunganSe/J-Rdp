using Core.Constants;
using Core.Workers;
using NLog;

namespace Core.Main;

public class Controller
{
    private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private int _pollingInterval = ConfigConstants.PollingInterval_Default;
    private readonly Worker _worker = new();
    private readonly ConfigWatcherWorker _configWatcherWorker = new();

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
        _worker.UpdateConfig();
        SetPollingInterval();
        _worker.SetDeleteDelay(_worker.GetDeleteDelay());
        InitializeProfiles();
    }

    private void SetPollingInterval()
    {
        int newPollingInterval = _worker.GetPollingInterval();
        if (newPollingInterval == _pollingInterval)
            return;

        _pollingInterval = newPollingInterval;
        _logger.Info($"Polling interval set to {_pollingInterval} ms.");
    }

    private void InitializeProfiles()
    {
        _worker.UpdateProfileInfos();
        _worker.UpdateProfileInfosFiles();
        _worker.LogProfileInfosSummary();
    }

    private void MainLoop()
    {
        _worker.UpdateProfileInfosFiles();
        _worker.ProcessProfileInfos();
    }
}
