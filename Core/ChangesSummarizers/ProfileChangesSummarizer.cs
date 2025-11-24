using Core.Profiles;

namespace Core.ChangesSummarizers;

internal static class ProfileChangesSummarizer
{
    public static List<string> GetChangedProfilesSettings(List<Profile> oldProfiles, List<Profile> newProfiles)
    {
        var keptProfiles = newProfiles
            .Where(np => oldProfiles.Any(op => op.Id == np.Id))
            .ToList();
        var removedProfiles = oldProfiles
            .Where(op => newProfiles.All(np => np.Id != op.Id))
            .ToList();
        var addedProfiles = newProfiles
            .Where(np => oldProfiles.All(op => op.Id != np.Id))
            .ToList();

        var output = new List<string>();

        if (addedProfiles.Count > 0)
            output.AddRange(addedProfiles.Select(np => $"Profile added: {np.Name}"));

        if (removedProfiles.Count > 0)
            output.AddRange(removedProfiles.Select(rp => $"Profile removed: {rp.Name}"));

        foreach (var keptProfile in keptProfiles)
        {
            var oldProfile = oldProfiles.First(op => op.Id == keptProfile.Id);
            var profileChanges = GetChangesSummary(oldProfile, keptProfile);
            output.AddRange(profileChanges.Select(change => $"Profile '{keptProfile.Name}': {change}"));
        }

        return output;
    }
}
