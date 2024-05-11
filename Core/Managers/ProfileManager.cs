using Core.Configuration;
using NLog;

namespace Core.Managers;

internal class ProfileManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public List<ProfileInfo> ProfileInfos { get; private set; } = [];

    public void UpdateProfileInfos(List<Profile> profiles)
        => ProfileInfos = profiles
            .Select(profile => new ProfileInfo(profile))
            .ToList();

    public void UpdateProfileInfosFiles()
        => ProfileInfos.ForEach(pi => pi.UpdateFiles());

    public void LogProfileInfosSummary()
    {
        var profileSummaries = ProfileInfos.Select(pi => $"\n  {pi.Profile.Name}: '{pi.Profile.Filter}' in: {pi.DirectoryFullPath}");
        string joinedSummaries = ProfileInfos.Count > 0
            ? string.Join("", profileSummaries)
            : "(none)";
        _logger.Info($"Current profiles: {joinedSummaries}");
    }
}
