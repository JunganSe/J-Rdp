using NLog;

namespace Core.Profiles;

internal class ProfileManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public List<ProfileWrapper> ProfileWrappers { get; private set; } = [];

    public void UpdateProfiles(List<Profile> profiles) =>
        ProfileWrappers = profiles
            .Select(profile => new ProfileWrapper(profile))
            .ToList();

    public void UpdateFilesInProfileWrappers() =>
        ProfileWrappers.ForEach(pw => pw.UpdateFiles());

    public void LogProfilesSummary()
    {
        var profileSummaries = ProfileWrappers.Select(pw => $"\n  {pw.Profile.Name}: '{pw.Profile.Filter}' in: {pw.DirectoryFullPath}");
        string joinedSummaries = ProfileWrappers.Count > 0
            ? string.Join("", profileSummaries)
            : "(none)";
        _logger.Info($"Current profiles: {joinedSummaries}");
    }
}
