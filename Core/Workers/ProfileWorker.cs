using Core.Configuration;
using NLog;

namespace Core.Workers;

internal class ProfileWorker
{
    private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private List<ProfileInfo> _profileInfos = [];

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
}
