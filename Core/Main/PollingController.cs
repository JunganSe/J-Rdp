using Core.Configuration;
using Core.Constants;
using Core.Extensions;
using NLog;

namespace Core.Main;

public class PollingController
{
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
            var config = configInfo.Config;

            if (newFiles.Any())
                _logger.Trace($"{config.Name} found {newFiles.Count()} new files in: {config.WatchFolder}");

            foreach (var newFile in newFiles)
                ProcessFileOnFilterMatch(config, newFile);
        }
    }



    private int GetValidPollingInterval(int pollingInterval)
    {
        if (pollingInterval is >= PollingInterval.Min and <= PollingInterval.Max)
            return pollingInterval;

        _logger.Warn($"Invalid polling interval ({pollingInterval}), defaulting to {PollingInterval.Default} ms.");
        return PollingInterval.Default;
    }

    private void UpdateConfigInfos()
        => _configInfos = _configManager.Configs
            .Select(config => new ConfigInfo(config))
            .ToList();

    private void UpdateConfigInfosFiles()
        => _configInfos.ForEach(ci => ci.UpdateFiles());

    private void ProcessFileOnFilterMatch(Config config, FileInfo file)
    {
        if (file.NameMatchesFilter(config.Filter, ignoreCase: true))
        {
            _logger.Info($"{config.Name} found a match on '{file.FullName}' using filter '{config.Filter}'.");
            _fileManager.ProcessFile(file, config);
        }
    }
}
