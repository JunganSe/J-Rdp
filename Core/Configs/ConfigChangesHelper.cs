using Core.Profiles;
using System.Runtime.CompilerServices;

namespace Core.Configs;

internal static class ConfigChangesHelper
{
    public static List<string> GetChangedSettings(Config oldConfig, Config newConfig)
    {
        var changedConfigSettings = GetChangedConfigSettings(oldConfig, newConfig);
        var changedProfileSettings = GetChangedProfilesSettings(oldConfig, newConfig);

        return [.. changedConfigSettings, .. changedProfileSettings];
    }

    public static List<string> GetChangedConfigSettings(Config oldConfig, Config newConfig)
    {
        var changedSettings = new List<string>();

        if (newConfig.PollingInterval != oldConfig.PollingInterval)
            changedSettings.Add(GetChangeSummary(oldConfig.PollingInterval, newConfig.PollingInterval));

        if (newConfig.DeleteDelay != oldConfig.DeleteDelay)
            changedSettings.Add(GetChangeSummary(oldConfig.DeleteDelay, newConfig.DeleteDelay));

        if (newConfig.ShowLog != oldConfig.ShowLog)
            changedSettings.Add(GetChangeSummary(oldConfig.ShowLog, newConfig.ShowLog));

        if (newConfig.LogToFile != oldConfig.LogToFile)
            changedSettings.Add(GetChangeSummary(oldConfig.LogToFile, newConfig.LogToFile));

        return changedSettings;
    }

    private static string GetChangeSummary<T>(T oldValue, T newValue,
        [CallerArgumentExpression(nameof(newValue))] string? newValueExpression = null) // Generates a string from the argument used when calling, e.g. "newConfig.PollingInterval"
    {
        string propertyName = newValueExpression?.Split('.').LastOrDefault() ?? "(Unknown)";
        return $"{propertyName}: {oldValue} => {newValue}";
    }

    public static List<string> GetChangedProfilesSettings(Config oldConfig, Config newConfig)
    {
        // TODO: Implement profile comparison logic.
        throw new NotImplementedException();
    }
}
