using Core.FileHandling;

namespace Core.ProfileHandling;

internal static class ProfileValidator
{
    public static bool IsProfileValid(Profile profile, out string reason)
    {
        var reasons = new List<string>();

        if (string.IsNullOrWhiteSpace(profile.WatchFolder))
            reasons.Add($"'{nameof(profile.WatchFolder)}' is empty or missing.");
        else if (!FileHelper.IsPathAbsolute(profile.WatchFolder))
            reasons.Add($"'{nameof(profile.WatchFolder)}' path is not absolute.");

        if (string.IsNullOrWhiteSpace(profile.Filter))
            reasons.Add($"'{nameof(profile.Filter)}' is empty or missing.");

        foreach (var setting in profile.Settings)
        {
            if (!IsProfileSettingValid(setting, out string settingReason))
                reasons.Add(settingReason);
        }

        reason = string.Join(" ", reasons);
        bool isValid = reasons.Count == 0;
        return isValid;
    }

    public static bool IsProfileSettingValid(string setting, out string reason)
    {
        var reasons = new List<string>();
        var parts = setting.Trim().Split(':');

        if (parts.Length < 2)
            reasons.Add($"Setting '{setting}' does not contain a semicolon.");
        else
        {
            if (parts[0].Length == 0)
                reasons.Add($"Setting '{setting}' has an empty key.");
            if (parts[^1].Length == 0)
                reasons.Add($"Setting '{setting}' has an empty value.");
        }

        reason = string.Join(" ", reasons);
        bool isValid = reasons.Count == 0;
        return isValid;
    }
}
