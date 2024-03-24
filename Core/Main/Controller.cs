using Core.Configuration;
using Core.Constants;
using Core.Extensions;
using Core.Helpers;
using NLog;

namespace Core.Main;

public class Controller
{
    private readonly int _pollingInterval;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly FileManager _fileManager = new();
    private readonly ConfigManager _configManager = new();
    private readonly List<string> _processedFilePaths = [];
    private List<ConfigInfo> _configInfos = [];

    public Controller(int pollingInterval)
    {
        _pollingInterval = GetValidPollingInterval(pollingInterval);
        _logger.Info($"Poll rate set to {_pollingInterval} ms.");
    }

    public void Run()
    {
        try
        {
            _logger.Info("Starting...");
            StartConfigWatcher();
            Initialize();
            _logger.Info("Started.");

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

    private void StartConfigWatcher()
    {
        string directory = FileSystemHelper.GetConfigDirectory();
        string fileName = ConfigManager.CONFIG_FILE_NAME;
        _ = new ConfigWatcher(directory, fileName, Initialize);
    }

    private void Initialize()
    {
        _configManager.UpdateConfigs();
        UpdateConfigInfos();
        UpdateConfigInfosFiles();
        LogConfigSummary();
    }

    private void MainLoop()
    {
        UpdateConfigInfosFiles();

        var configInfos = _configInfos.Where(ci => ci.DirectoryExists);
        foreach (var configInfo in configInfos)
            ProcessNewFiles(configInfo);

        _processedFilePaths.Clear();
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

    private void LogNewFiles(Config config, IEnumerable<FileInfo> newFiles)
    {
        string pluralS = (newFiles.Count() > 1) ? "s" : "";
        string fileNames = string.Join("", newFiles.Select(f => $"\n  {f.Name}"));
        _logger.Trace($"{config.Name} found {newFiles.Count()} new file{pluralS} in '{config.WatchFolder}': {fileNames}");
    }

    private void LogConfigSummary()
    {
        string configsSummary = (_configInfos.Count > 0)
            ? string.Join("", _configInfos
                .Select(ci => ci.Config)
                .Select(c => $"\n  {c.Name}: '{c.Filter}' in: {c.WatchFolder}"))
            : "(nothing)";
        _logger.Info($"Current configs: {configsSummary}");
    }

    private void ProcessNewFiles(ConfigInfo configInfo)
    {
        var newFiles = configInfo.NewFiles
            .Where(file => !_processedFilePaths.Contains(file.FullName));

        if (!newFiles.Any())
            return;

        LogNewFiles(configInfo.Config, newFiles);

        foreach (var newFile in newFiles)
            ProcessFileOnFilterMatch(configInfo.Config, newFile);
    }

    private void ProcessFileOnFilterMatch(Config config, FileInfo file)
    {
        if (!file.NameMatchesFilter(config.Filter, ignoreCase: true))
            return;

        _logger.Info($"{config.Name} found a match on '{file.FullName}' using filter '{config.Filter}'.");

        _processedFilePaths.Add(file.FullName);
        _fileManager.ProcessFile(file, config);
    }
}
