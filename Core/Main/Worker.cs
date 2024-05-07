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
    private List<ProfileInfo> _profileInfos = [];

    #region ConfigWatcher

    #endregion

    #region Config

    

    #endregion

    #region Profile

    public List<ProfileInfo> GetProfileInfos() 
        => _profileInfos;

    public void UpdateProfileInfos(List<Profile> profiles)
        => _profileInfos = profiles
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

    #endregion

    #region File

    #endregion
}
