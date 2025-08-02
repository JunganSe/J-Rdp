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
        var profileSummaries = ProfileWrappers
            .Select(pw => $"\n  {pw.Profile.Name}: '{pw.Profile.Filter}' in: {pw.DirectoryFullPath}");
        string joinedSummaries = (ProfileWrappers.Count > 0)
            ? string.Join("", profileSummaries)
            : "(none)";
        _logger.Info($"Current profiles: {joinedSummaries}");
    }

    public void LogProfilesSummaryIfChanged(List<Profile> previousProfiles)
    {
        var currentProfiles = ProfileWrappers.Select(pw => pw.Profile).ToList();
        bool areProfilesEquivalent = AreProfilesEquivalent(currentProfiles, previousProfiles);
        if (areProfilesEquivalent)
            return;

        var profileSummaries = ProfileWrappers
            .Select(pw => $"\n  {pw.Profile.Name}: '{pw.Profile.Filter}' in: {pw.DirectoryFullPath}");
        string joinedSummaries = (ProfileWrappers.Count > 0)
            ? string.Join("", profileSummaries)
            : "(none)";
        _logger.Info($"Current profiles: {joinedSummaries}");
    }

    private bool AreProfilesEquivalent(List<Profile> profilesA, List<Profile> profilesB)
    {
        if (profilesA.Count != profilesB.Count)
            return false;

        return profilesA.All(a =>
            profilesB.Any(b => ProfileHelper.AreProfilesEqual(b, a)));
    }
}
