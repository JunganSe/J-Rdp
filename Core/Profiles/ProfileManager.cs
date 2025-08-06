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

    public void LogProfilesSummaryIfChanged(List<Profile> previousProfiles)
    {
        if (AreProfilesEquivalentToCurrent(previousProfiles))
            return;

        LogProfilesSummary();
    }

    private bool AreProfilesEquivalentToCurrent(List<Profile> profiles)
    {
        var currentProfiles = ProfileWrappers.Select(pw => pw.Profile).ToList();
        var comparer = new EqualityComparer_Profile_AllExceptId();
        return currentProfiles.SequenceEqual(profiles, comparer);
    }

    private void LogProfilesSummary()
    {
        var profileSummaries = ProfileWrappers
            .Select(pw => $"\n  {pw.Profile.Name}: '{pw.Profile.Filter}' in: {pw.DirectoryFullPath}");
        string joinedSummaries = (ProfileWrappers.Count > 0)
            ? string.Join("", profileSummaries)
            : "(none)";
        _logger.Info($"Profiles were changed. Current profiles: {joinedSummaries}");
    }
}
