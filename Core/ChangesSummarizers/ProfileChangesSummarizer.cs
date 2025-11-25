using Core.Profiles;
using System.Runtime.CompilerServices;

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
            output.AddRange(addedProfiles.Select(np => $"Profile '{np.Name}' added."));

        if (removedProfiles.Count > 0)
            output.AddRange(removedProfiles.Select(rp => $"Profile '{rp.Name}' removed."));

        foreach (var keptProfile in keptProfiles)
        {
            var oldProfile = oldProfiles.First(op => op.Id == keptProfile.Id);
            var profileChanges = GetChangedProfileSettings(oldProfile, keptProfile);
            output.AddRange(profileChanges.Select(change => $"Profile '{keptProfile.Name}' changed. {change}"));
        }

        return output;
    }

    public static List<string> GetChangedProfileSettings(Profile oldProfile, Profile newProfile)
    {
        var changes = new List<string>();

        if (newProfile.Name != oldProfile.Name)
            changes.Add(GetChangeSummary(oldProfile.Name, newProfile.Name));

        if (newProfile.Enabled != oldProfile.Enabled)
            changes.Add(GetChangeSummary(oldProfile.Enabled, newProfile.Enabled));

        if (newProfile.WatchFolder != oldProfile.WatchFolder)
            changes.Add(GetChangeSummary(oldProfile.WatchFolder, newProfile.WatchFolder));

        if (newProfile.Filter != oldProfile.Filter)
            changes.Add(GetChangeSummary(oldProfile.Filter, newProfile.Filter));

        if (newProfile.MoveToFolder != oldProfile.MoveToFolder)
            changes.Add(GetChangeSummary(oldProfile.MoveToFolder, newProfile.MoveToFolder));

        if (newProfile.Launch != oldProfile.Launch)
            changes.Add(GetChangeSummary(oldProfile.Launch, newProfile.Launch));

        if (newProfile.Delete != oldProfile.Delete)
            changes.Add(GetChangeSummary(oldProfile.Delete, newProfile.Delete));

        var settingsChanges = GetSettingsChanges(oldProfile.Settings, newProfile.Settings);
        changes.AddRange(settingsChanges);

        return changes;
    }

    private static string GetChangeSummary<T>(T oldValue, T newValue,
        [CallerArgumentExpression(nameof(newValue))] string? newValueExpression = null) // Generates a string from the argument used when calling, e.g. "newConfig.PollingInterval"
    {
        string propertyName = newValueExpression?.Split('.').LastOrDefault() ?? "(Unknown)";
        return $"{propertyName}: {oldValue} => {newValue}";
    }

    public static List<string> GetSettingsChanges(List<string> oldSettings, List<string> newSettings)
    {
        bool areSettingsEqual = newSettings.Order().SequenceEqual(oldSettings.Order());
        if (areSettingsEqual)
            return [];

        var oldSettingsLookup = oldSettings.ToDictionary(s => s.Split(':')[0]);
        var newSettingsLookup = newSettings.ToDictionary(s => s.Split(':')[0]);

        var addedSettings = newSettingsLookup.Keys
            .Where(key => !oldSettingsLookup.ContainsKey(key))
            .Select(key => newSettingsLookup[key])
            .ToList();
        var removedSettings = oldSettingsLookup.Keys
            .Where(key => !newSettingsLookup.ContainsKey(key))
            .Select(key => oldSettingsLookup[key])
            .ToList();
        var changedSettings = newSettingsLookup.Keys
            .Where(key => oldSettingsLookup.ContainsKey(key) && oldSettingsLookup[key] != newSettingsLookup[key])
            .Select(key => (oldSetting: oldSettingsLookup[key], newSetting: newSettingsLookup[key]))
            .ToList();

        var output = new List<string>();

        if (addedSettings.Count > 0)
        {
            string settingsWord = (addedSettings.Count == 1) ? "setting" : "settings";
            string settings = string.Join(", ", addedSettings.Select(s => $"'{s}'"));
            output.Add($"Added {settingsWord}: {settings}");
        }

        if (removedSettings.Count > 0)
        {
            string settingsWord = (removedSettings.Count == 1) ? "setting" : "settings";
            string settings = string.Join(", ", removedSettings.Select(s => $"'{s}'"));
            output.Add($"Removed {settingsWord}: {settings}");
        }

        foreach (var (oldSetting, newSetting) in changedSettings)
            output.Add($"Changed setting: '{oldSetting}' => '{newSetting}'");

        return output;
    }
}
