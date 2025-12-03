namespace Core.ChangesSummarizers;

public class ProfileSettingsChangesSummarizer
{
    public static List<string> GetSettingsChanges(List<string> oldSettings, List<string> newSettings)
    {
        bool areSettingsEqual = newSettings.Order().SequenceEqual(oldSettings.Order());
        if (areSettingsEqual)
            return [];

        var output = new List<string>();
        var (addedSettings, removedSettings, changedSettings) = GetGroupedSettings(oldSettings, newSettings);

        if (addedSettings.Count > 0)
            output.Add("Added " + GetJoinedSettingsSummary(addedSettings));

        if (removedSettings.Count > 0)
            output.Add("Removed " + GetJoinedSettingsSummary(removedSettings));

        foreach (var (oldSetting, newSetting) in changedSettings)
            output.Add($"Changed setting: '{oldSetting}' => '{newSetting}'");

        return output;
    }

    /// <summary>
    /// Compares two lists of settings and categorizes the differences into added, removed, and changed settings.
    /// </summary>
    /// <remarks>This method assumes that all settings are valid. This means each setting is formatted as "key:value",
    /// where "key" is a unique identifier. The comparison is performed based on the "key" portion of each setting.</remarks>
    public static (List<string> added, List<string> removed, List<(string oldSetting, string newSetting)> changed) GetGroupedSettings(List<string> oldSettings, List<string> newSettings)
    {
        ThrowIfAnySettingIsInvalid([.. oldSettings, .. newSettings]);

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

        return (addedSettings, removedSettings, changedSettings);
    }

    public static void ThrowIfAnySettingIsInvalid(List<string> settings)
    {
        var splitOptions = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        var invalidSettings = settings
            .Where(s =>
                !s.Contains(':')
                || s.Split(':', splitOptions).Length != 2)
            .ToList();

        if (invalidSettings.Count > 0)
            throw new ArgumentException("All settings must be in the format 'key:value'. Invalid settings: "
                + string.Join(", ", invalidSettings));
    }

    public static string GetJoinedSettingsSummary(List<string> settings)
    {
        string settingsWord = (settings.Count == 1) ? "setting" : "settings";
        string joinedSettings = string.Join(", ", settings.Select(s => $"'{s}'"));
        return $"{settingsWord}: {joinedSettings}";
    }
}
