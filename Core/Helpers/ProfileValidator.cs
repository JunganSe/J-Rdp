using Core.Models;

namespace Core.Helpers;

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

        reason = string.Join(" ", reasons);
        return (reasons.Count == 0);
    }
}
