using Core.Configuration;
using Core.Constants;
using Core.Extensions;
using Core.Helpers;
using NLog;

namespace Core.Main;

public class Controller
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly FileManager _fileManager = new();
    private readonly ConfigManager _configManager = new();
    private readonly List<string> _processedFilePaths = [];
    private List<ConfigInfo> _configInfos = [];
    private int _pollingInterval;



    #region Main

    public void Run(int pollingInterval)
    {
        try
        {
            Initialize(pollingInterval);

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
    }

    private void Initialize(int pollingInterval)
    {
        _logger.Trace("Starting...");

        _pollingInterval = GetValidPollingInterval(pollingInterval);
        StartConfigWatcher();
        InitializeConfigs();

        _logger.Info($"Running at poll rate {_pollingInterval} ms.");
    }

    private void MainLoop()
    {
        UpdateConfigInfosFiles();

        var configInfos = _configInfos.Where(ci => ci.DirectoryExists);
        foreach (var configInfo in configInfos)
            ProcessNewFiles(configInfo);

        _processedFilePaths.Clear();
    }

    private void ProcessNewFiles(ConfigInfo configInfo)
    {
        var newFiles = configInfo.NewFiles.Where(file => !_processedFilePaths.Contains(file.FullName));

        if (!newFiles.Any())
            return;

        LogNewFiles(configInfo.Config, newFiles);

        foreach (var newFile in newFiles)
            ProcessFileOnFilterMatch(configInfo.Config, newFile);
    }

    #endregion


    #region Other

    private int GetValidPollingInterval(int pollingInterval)
    {
        if (pollingInterval is >= PollingInterval.Min and <= PollingInterval.Max)
            return pollingInterval;

        _logger.Warn($"Invalid polling interval ({pollingInterval}), defaulting to {PollingInterval.Default} ms. (Must be {PollingInterval.Min}-{PollingInterval.Max} ms.)");
        return PollingInterval.Default;
    }

    private void StartConfigWatcher()
    {
        string directory = FileSystemHelper.GetConfigDirectory();
        string fileName = ConfigManager.CONFIG_FILE_NAME;
        _ = new ConfigWatcher(directory, fileName, callback: InitializeConfigs);
    }

    private void InitializeConfigs()
    {
        _configManager.UpdateConfigs();
        UpdateConfigInfos();
        UpdateConfigInfosFiles();
        LogConfigSummary();
    }

    private void UpdateConfigInfos()
        => _configInfos = _configManager.Configs
            .Select(config => new ConfigInfo(config))
            .ToList();

    private void UpdateConfigInfosFiles()
        => _configInfos.ForEach(ci => ci.UpdateFiles());

    private void LogConfigSummary()
    {
        string configsSummary = (_configInfos.Count > 0)
            ? string.Join("", _configInfos
                .Select(ci => $"\n  {ci.Config.Name}: '{ci.Config.Filter}' in: {ci.DirectoryFullPath}"))
            : "(none)";
        _logger.Info($"Current configs: {configsSummary}");
    }

    private void LogNewFiles(Config config, IEnumerable<FileInfo> newFiles)
    {
        string s = (newFiles.Count() > 1) ? "s" : "";
        string fileNames = string.Join("", newFiles.Select(f => $"\n  {f.Name}"));
        _logger.Debug($"'{config.Name}' found {newFiles.Count()} new file{s} in '{config.WatchFolder}': {fileNames}");
    }

    private void ProcessFileOnFilterMatch(Config config, FileInfo file)
    {
        if (!file.NameMatchesFilter(config.Filter, ignoreCase: true))
            return;

        _logger.Info($"{config.Name} found a match on '{file.FullName}' using filter '{config.Filter}'.");

        _processedFilePaths.Add(file.FullName);
        _fileManager.ProcessFile(file, config);
    }

    #endregion
}
