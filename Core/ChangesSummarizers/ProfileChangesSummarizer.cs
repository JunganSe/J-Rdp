using Core.Profiles;

namespace Core.ChangesSummarizers;

internal static class ProfileChangesSummarizer
{
    public static List<string> GetChangedProfilesSettings(List<Profile> oldProfiles, List<Profile> newProfiles)
    {
        var addedProfiles = newProfiles
            .Where(np => oldProfiles.All(op => op.Id != np.Id))
            .ToList();
        var removedProfiles = oldProfiles
            .Where(op => newProfiles.All(np => np.Id != op.Id))
            .ToList();
        var keptProfiles = newProfiles
            .Where(np => oldProfiles.Any(op => op.Id == np.Id))
            .ToList();

        var output = new List<string>();

        if (addedProfiles.Count > 0)
            output.AddRange(addedProfiles.Select(np => $"Profile added: '{np.Name}'"));

        if (removedProfiles.Count > 0)
            output.AddRange(removedProfiles.Select(rp => $"Profile removed: '{rp.Name}'"));

        foreach (var keptProfile in keptProfiles)
        {
            var oldProfile = oldProfiles.First(op => op.Id == keptProfile.Id);
            var profileChanges = GetChangedProfileSettings(oldProfile, keptProfile);
            output.AddRange(profileChanges.Select(change => $"Profile changed: '{keptProfile.Name}': {change}"));
        }

        return output;
    }

    public static List<string> GetChangedProfileSettings(Profile oldProfile, Profile newProfile)
    {
        // TODO: Implement.
        return ["Dummy string A", "Dummy string B"];
    }
}
