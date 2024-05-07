using Auxiliary;
using Core.Configuration;
using Core.Constants;
using Core.Extensions;
using Core.Helpers;
using NLog;

namespace Core.Main;

internal class Worker
{
    private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly RdpManager _rdpManager = new();
    private readonly ConfigManager _configManager = new();
    private readonly List<string> _processedFilePaths = [];
    private ConfigWatcher? _configWatcher;
    private List<ProfileInfo> _profileInfos = [];

    public void StopAndDisposeConfigWatcher()
    {
        try
        {
            if (_configWatcher == null)
                return;

            _configWatcher.EnableRaisingEvents = false;
            _configWatcher.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Swallow exception if it's already disposed.
        }
    }

    public void StartConfigWatcher(Action callback)
    {
        string directory = FileHelper.GetConfigDirectory();
        string fileName = ConfigConstants.FileName;
        _configWatcher = new ConfigWatcher(directory, fileName, callback: callback);
    }

    public void UpdateConfig()
        => _configManager.UpdateConfig();

    public int GetPollingInterval() 
        => MathExt.Median(_configManager.Config.PollingInterval, ConfigConstants.PollingInterval_Min, ConfigConstants.PollingInterval_Max);

    public void SetDeleteDelay()
    {
        int newDeleteDelay = MathExt.Median(_configManager.Config.DeleteDelay, ConfigConstants.DeleteDelay_Min, ConfigConstants.DeleteDelay_Max);
        if (newDeleteDelay == _rdpManager.DeleteDelay)
            return;

        _rdpManager.DeleteDelay = newDeleteDelay;
        _logger.Info($"Delete delay set to {_rdpManager.DeleteDelay} ms.");
    }

    public void UpdateProfileInfos()
        => _profileInfos = _configManager.Config.Profiles
            .Select(profile => new ProfileInfo(profile))
            .ToList();

    public void UpdateProfileInfosFiles()
        => _profileInfos.ForEach(ci => ci.UpdateFiles());

    public void LogProfileInfosSummary()
    {
        var profileSummaries = _profileInfos.Select(pi => $"\n  {pi.Profile.Name}: '{pi.Profile.Filter}' in: {pi.DirectoryFullPath}");
        string joinedSummaries = (_profileInfos.Count > 0)
            ? string.Join("", profileSummaries)
            : "(none)";
        _logger.Info($"Current profiles: {joinedSummaries}");
    }

    public void ProcessProfileInfos()
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

    private void LogNewFiles(Profile profile, IEnumerable<FileInfo> newFiles)
    {
        string s = (newFiles.Count() > 1) ? "s" : "";
        string fileNames = string.Join("", newFiles.Select(f => $"\n  {f.Name}"));
        _logger.Debug($"'{profile.Name}' found {newFiles.Count()} new file{s} in '{profile.WatchFolder}': {fileNames}");
    }

    private void ProcessFileOnFilterMatch(FileInfo file, Profile profile)
    {
        if (!file.NameMatchesFilter(profile.Filter, ignoreCase: true))
            return;

        _logger.Info($"'{profile.Name}' found a match on '{file.FullName}' using filter '{profile.Filter}'.");

        _processedFilePaths.Add(file.FullName);
        _rdpManager.ProcessFile(file, profile);
    }
}
