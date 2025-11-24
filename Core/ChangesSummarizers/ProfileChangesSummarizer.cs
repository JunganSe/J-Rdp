using Core.Configs;

namespace Core.ChangesSummarizers;

internal static class ProfileChangesSummarizer
{
    public static List<string> GetChangedProfilesSettings(Config oldConfig, Config newConfig)
    {
        var keptProfiles = newConfig.Profiles
            .Where(np => oldConfig.Profiles.Any(op => op.Id == np.Id))
            .ToList();
        var removedProfiles = oldConfig.Profiles
            .Where(op => newConfig.Profiles.All(np => np.Id != op.Id))
            .ToList();
        var newProfiles = newConfig.Profiles
            .Where(np => oldConfig.Profiles.All(op => op.Id != np.Id))
            .ToList();

        var output = new List<string>();

        if (newProfiles.Count > 0)
            output.AddRange(newProfiles.Select(np => $"Profile added: {np.Name}"));

        if (removedProfiles.Count > 0)
            output.AddRange(removedProfiles.Select(rp => $"Profile removed: {rp.Name}"));

        foreach (var keptProfile in keptProfiles)
        {
            var oldProfile = oldConfig.Profiles.First(op => op.Id == keptProfile.Id);
            var profileChanges = GetChangesSummary(oldProfile, keptProfile);
            output.AddRange(profileChanges.Select(change => $"Profile '{keptProfile.Name}': {change}"));
        }

        return output;
    }
}
