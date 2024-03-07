using Core.Configuration;
using Core.Extensions;
using NLog;

namespace Core.Main;

public class PollingController
{
    private const int _defaultPollingInterval = 1000;

    private readonly int _pollingInterval;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly FileManager _fileManager = new();
    private readonly ConfigManager _configManager = new();
    private List<ConfigInfo> _configInfos = [];

    public PollingController(int pollingInterval)
    {
        _pollingInterval = GetValidPollingInterval(pollingInterval);
    }

    public void Run()
    {
        try
        {
            _logger.Info("Starting...");
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
        }
        finally
        {
            _logger.Info("Quitting...");
        }
    }

    private void Initialize()
    {
        _configManager.UpdateConfigs();
        UpdateConfigInfos();
        UpdateConfigInfosFiles();
    }

    private void MainLoop()
    {
        UpdateConfigInfosFiles();

        var configInfos = _configInfos.Where(ci => ci.DirectoryExists);
        foreach (var configInfo in configInfos)
        {
            var newFiles = configInfo.NewFiles;
            if (newFiles.Any())
                _logger.Trace($"{configInfo.Config.Name} found {newFiles.Count()} new files in: {configInfo.Config.WatchFolder}");

            string filter = configInfo.Config.Filter;
            foreach (var newFile in newFiles)
            {
                if (newFile.NameMatchesFilter(filter, ignoreCase: true))
                {
                    _logger.Info($"{configInfo.Config.Name} found a match on '{newFile.FullName}' using filter '{configInfo.Config.Filter}'.");
                    _fileManager.ProcessFile(newFile, configInfo.Config);
                }
            }
        }
    }



    private int GetValidPollingInterval(int pollingInterval)
    {
        if (pollingInterval > 0)
            return pollingInterval;

        _logger.Warn($"Invalid polling interval ({pollingInterval}), defaulting to {_defaultPollingInterval}");
        return _defaultPollingInterval;
    }

    private void UpdateConfigInfos()
        => _configInfos = _configManager.Configs
            .Select(config => new ConfigInfo(config))
            .ToList();

    private void UpdateConfigInfosFiles()
        => _configInfos.ForEach(ci => ci.UpdateFiles());
}
