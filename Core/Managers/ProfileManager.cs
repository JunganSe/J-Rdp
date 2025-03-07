using Core.Models;
using NLog;

namespace Core.Managers;

internal class ProfileManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public List<ProfileWrapper> ProfileWrappers { get; private set; } = [];

    public void UpdateProfiles(List<Profile> profiles) =>
        ProfileWrappers = profiles
            .Select(profile => new ProfileWrapper(profile))
            .ToList();

    public void UpdateFiles() =>
        ProfileWrappers.ForEach(pi => pi.UpdateFiles());

    public void LogProfilesSummary()
    {
        var profileSummaries = ProfileWrappers.Select(pi => $"\n  {pi.Profile.Name}: '{pi.Profile.Filter}' in: {pi.DirectoryFullPath}");
        string joinedSummaries = ProfileWrappers.Count > 0
            ? string.Join("", profileSummaries)
            : "(none)";
        _logger.Info($"Current profiles: {joinedSummaries}");
    }
}
