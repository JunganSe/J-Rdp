namespace Core.Configs;

internal static class ConfigChangesHelper
{
    public static List<string> GetChangedSettings(Config oldConfig, Config newConfig)
    {
        var changedSettings = new List<string>();

        if (newConfig.PollingInterval != oldConfig.PollingInterval)
            changedSettings.Add(GetChangeSummary(nameof(Config.PollingInterval), oldConfig.PollingInterval, newConfig.PollingInterval));

        if (newConfig.DeleteDelay != oldConfig.DeleteDelay)
            changedSettings.Add(GetChangeSummary(nameof(Config.DeleteDelay), oldConfig.DeleteDelay, newConfig.DeleteDelay));

        if (newConfig.ShowLog != oldConfig.ShowLog)
            changedSettings.Add(GetChangeSummary(nameof(Config.ShowLog), oldConfig.ShowLog, newConfig.ShowLog));

        if (newConfig.LogToFile != oldConfig.LogToFile)
            changedSettings.Add(GetChangeSummary(nameof(Config.LogToFile), oldConfig.LogToFile, newConfig.LogToFile));

        // TODO: Compare Profiles.

        return changedSettings;
    }

    private static string GetChangeSummary<T>(string propertyName, T oldValue, T newValue) =>
        $"{propertyName}: {oldValue} => {newValue}";
}
