namespace Core.ChangesSummarizers;

public class ProfileSettingsChangesSummarizer
{
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
            output.Add("Added " + GetJoinedSettingsSummary(addedSettings));

        if (removedSettings.Count > 0)
            output.Add("Removed " + GetJoinedSettingsSummary(removedSettings));

        foreach (var (oldSetting, newSetting) in changedSettings)
            output.Add($"Changed setting: '{oldSetting}' => '{newSetting}'");

        return output;
    }

    public static string GetJoinedSettingsSummary(List<string> settings)
    {
        string settingsWord = (settings.Count == 1) ? "setting" : "settings";
        string joinedSettings = string.Join(", ", settings.Select(s => $"'{s}'"));
        return $"{settingsWord}: {joinedSettings}";
    }
}
