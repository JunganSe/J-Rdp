using Core.Configuration;
using Core.Constants;
using Core.Extensions;
using Core.Helpers;
using NLog;

namespace Core.Main;

public class Controller
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly RdpManager _rdpManager = new();
    private readonly ConfigManager _configManager = new();
    private readonly List<string> _processedFilePaths = [];
    private List<ProfileInfo> _profileInfos = [];
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
        InitializeProfiles();

        _logger.Info($"Running at poll rate {_pollingInterval} ms.");
    }

    private void MainLoop()
    {
        UpdateProfileInfosFiles();

        var profileInfos = _profileInfos.Where(ci => ci.DirectoryExists);
        foreach (var profileInfo in profileInfos)
            ProcessNewFiles(profileInfo);

        _processedFilePaths.Clear();
    }

    private void ProcessNewFiles(ProfileInfo profileInfo)
    {
        var newFiles = profileInfo.NewFiles.Where(file => !_processedFilePaths.Contains(file.FullName));

        if (!newFiles.Any())
            return;

        LogNewFiles(profileInfo.Profile, newFiles);

        foreach (var newFile in newFiles)
            ProcessFileOnFilterMatch(profileInfo.Profile, newFile);
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
        string directory = FileHelper.GetConfigDirectory();
        string fileName = ConfigConstants.FileName;
        _ = new ConfigWatcher(directory, fileName, callback: InitializeProfiles);
    }

    private void InitializeProfiles()
    {
        _configManager.UpdateProfiles();
        UpdateProfileInfos();
        UpdateProfileInfosFiles();
        LogProfileSummary();
    }

    private void UpdateProfileInfos()
        => _profileInfos = _configManager.Profiles
            .Select(profile => new ProfileInfo(profile))
            .ToList();

    private void UpdateProfileInfosFiles()
        => _profileInfos.ForEach(ci => ci.UpdateFiles());

    private void LogProfileSummary()
    {
        string profilesSummary = (_profileInfos.Count > 0)
            ? string.Join("", _profileInfos
                .Select(ci => $"\n  {ci.Profile.Name}: '{ci.Profile.Filter}' in: {ci.DirectoryFullPath}"))
            : "(none)";
        _logger.Info($"Current profiles: {profilesSummary}");
    }

    private void LogNewFiles(Profile profile, IEnumerable<FileInfo> newFiles)
    {
        string s = (newFiles.Count() > 1) ? "s" : "";
        string fileNames = string.Join("", newFiles.Select(f => $"\n  {f.Name}"));
        _logger.Debug($"'{profile.Name}' found {newFiles.Count()} new file{s} in '{profile.WatchFolder}': {fileNames}");
    }

    private void ProcessFileOnFilterMatch(Profile profile, FileInfo file)
    {
        if (!file.NameMatchesFilter(profile.Filter, ignoreCase: true))
            return;

        _logger.Info($"'{profile.Name}' found a match on '{file.FullName}' using filter '{profile.Filter}'.");

        _processedFilePaths.Add(file.FullName);
        _rdpManager.ProcessFile(file, profile);
    }

    #endregion
}
