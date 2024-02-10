using Core.Configuration;
using Core.Extensions;
using NLog;

namespace Core.Main;

public class PollingController
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly int _pollingInterval = 1000;
    private readonly ConfigManager _configManager = new();
    private List<ConfigInfo> _configInfos = [];

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
        _logger.Info("Quitting...");
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

        var configInfos = _configInfos.Where(ci => ci.Directory.Exists);
        foreach (var configInfo in configInfos)
        {
            var newFiles = configInfo.NewFiles;
            if (newFiles.Any())
                _logger.Trace($"Found {newFiles.Count()} new files in: {configInfo.Directory.FullName}");
            
            string filter = configInfo.Config.Filter;
            foreach (var newFile in newFiles)
            {
                if (newFile.NameMatchesFilter(filter, ignoreCase: true))
                {
                    _logger.Info($"Filter match on: {newFile.FullName}");
                    // TODO: Take action on the file.
                }
            }
        }
    }




    private void UpdateConfigInfos() =>
        _configInfos = _configManager.Configs
            .Select(config => new ConfigInfo(config))
            .ToList();

    private void UpdateConfigInfosFiles() =>
        _configInfos.ForEach(ci => ci.UpdateFiles());
}
