using Core.Constants;
using Core.Workers;
using NLog;

namespace Core.Main;

public class Controller
{
    private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly ConfigWatcherWorker _configWatcherWorker = new();
    private readonly ConfigWorker _configWorker = new();
    private readonly ProfileWorker _profileWorker = new();
    private readonly FileWorker _fileWorker = new();
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
        _fileWorker.SetDeleteDelay(_configWorker.GetDeleteDelay());
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
        _profileWorker.UpdateProfileInfos(_configWorker.Profiles);
        _profileWorker.UpdateProfileInfosFiles();
        _profileWorker.LogProfileInfosSummary();
    }

    private void MainLoop()
    {
        _profileWorker.UpdateProfileInfosFiles();
        _fileWorker.ProcessProfileInfos(_profileWorker.ProfileInfos);
    }
}
