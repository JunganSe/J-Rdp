using Auxiliary;
using Core.Configuration;
using Core.Constants;
using Core.Extensions;
using Core.Helpers;
using NLog;

namespace Core.Main;

public class Controller
{
    private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly RdpManager _rdpManager = new();
    private readonly ConfigManager _configManager = new();
    private readonly List<string> _processedFilePaths = [];
    private List<ProfileInfo> _profileInfos = [];
    private int _pollingInterval;



    #region Main

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
        }
    }

    private void Initialize()
    {
        _logger.Trace("Starting...");

        SetDefaultPollingInterval();
        StartConfigWatcher();
        InitializeConfig();

        _logger.Info($"Running at poll rate {_pollingInterval} ms.");
    }

    private void MainLoop()
    {
        UpdateProfileInfosFiles();
        ProcessProfileInfos();
    }

    private void ProcessProfileInfos()
    {
        _processedFilePaths.Clear();
        var profileInfos = _profileInfos.Where(ci => ci.DirectoryExists);
        foreach (var profileInfo in profileInfos)
            ProcessNewFiles(profileInfo);
    }

    private void ProcessNewFiles(ProfileInfo profileInfo)
    {
        var newFiles = profileInfo.NewFiles.Where(file => !_processedFilePaths.Contains(file.FullName));

        if (!newFiles.Any())
            return;

        LogNewFiles(profileInfo.Profile, newFiles);

        foreach (var newFile in newFiles)
            ProcessFileOnFilterMatch(newFile, profileInfo.Profile);
    }

    private void ProcessFileOnFilterMatch(FileInfo file, Profile profile)
    {
        if (!file.NameMatchesFilter(profile.Filter, ignoreCase: true))
            return;

        _logger.Info($"'{profile.Name}' found a match on '{file.FullName}' using filter '{profile.Filter}'.");

        _processedFilePaths.Add(file.FullName);
        _rdpManager.ProcessFile(file, profile);
    }

    #endregion



    #region Other

    private void SetDefaultPollingInterval()
    {
        _pollingInterval = ConfigConstants.PollingInterval_Default;
        _logger.Info($"Polling interval set to {_pollingInterval} ms. (default)");
    }

    private void SetPollingInterval(int pollingInterval)
    {
        _pollingInterval = GetValidPollingInterval(pollingInterval);
        _logger.Info($"Polling interval set to {_pollingInterval} ms.");
    }

    private int GetValidPollingInterval(int pollingInterval)
    {
        int min = ConfigConstants.PollingInterval_Min;
        int max = ConfigConstants.PollingInterval_Max;
        int defaultInterval = ConfigConstants.PollingInterval_Default;

        if (pollingInterval >= min && pollingInterval <= max)
            return pollingInterval;

        _logger.Warn($"Invalid polling interval ({pollingInterval}), defaulting to {defaultInterval} ms. (Must be {min}-{max}.)");
        return defaultInterval;
    }

    private void SetDeleteDelayFromConfig()
    {
        int newDeleteDelay = MathExt.Median(_configManager.Config.DeleteDelay, ConfigConstants.DeleteDelay_Min, ConfigConstants.DeleteDelay_Max);
        if (newDeleteDelay == _rdpManager.DeleteDelay)
            return;

        _rdpManager.DeleteDelay = newDeleteDelay;
        _logger.Info($"Delete delay set to {_rdpManager.DeleteDelay} ms.");
    }

    private void StartConfigWatcher()
    {
        string directory = FileHelper.GetConfigDirectory();
        string fileName = ConfigConstants.FileName;
        _ = new ConfigWatcher(directory, fileName, callback: InitializeConfig);
    }

    private void InitializeConfig()
    {
        _configManager.UpdateConfig();

        int newPollingInterval = _configManager.Config.PollingInterval;
        if (newPollingInterval != _pollingInterval)
            SetPollingInterval(newPollingInterval);

        SetDeleteDelayFromConfig();

        InitializeProfiles();
    }

    private void InitializeProfiles()
    {
        UpdateProfileInfos();
        UpdateProfileInfosFiles();
        LogProfileInfosSummary();
    }

    private void UpdateProfileInfos()
        => _profileInfos = _configManager.Config.Profiles
            .Select(profile => new ProfileInfo(profile))
            .ToList();

    private void UpdateProfileInfosFiles()
        => _profileInfos.ForEach(ci => ci.UpdateFiles());

    private void LogProfileInfosSummary()
    {
        var profileSummaries = _profileInfos.Select(pi => $"\n  {pi.Profile.Name}: '{pi.Profile.Filter}' in: {pi.DirectoryFullPath}");
        string joinedSummaries = (_profileInfos.Count > 0)
            ? string.Join("", profileSummaries)
            : "(none)";
        _logger.Info($"Current profiles: {joinedSummaries}");
    }

    private void LogNewFiles(Profile profile, IEnumerable<FileInfo> newFiles)
    {
        string s = (newFiles.Count() > 1) ? "s" : "";
        string fileNames = string.Join("", newFiles.Select(f => $"\n  {f.Name}"));
        _logger.Debug($"'{profile.Name}' found {newFiles.Count()} new file{s} in '{profile.WatchFolder}': {fileNames}");
    }

    #endregion
}
