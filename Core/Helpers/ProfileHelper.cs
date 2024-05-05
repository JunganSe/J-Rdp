using Core.Configuration;

namespace Core.Helpers;

internal static class ProfileHelper
{
    public static bool IsProfileValid(Profile profile, out string reason)
    {
        var reasons = new List<string>();

        if (string.IsNullOrWhiteSpace(profile.WatchFolder))
            reasons.Add($"'{nameof(profile.WatchFolder)}' is empty.");
        else if (!FileHelper.IsPathAbsolute(profile.WatchFolder))
            reasons.Add($"'{nameof(profile.WatchFolder)}' path is not absolute.");

        reason = string.Join(" ", reasons);
        return reasons.Count == 0;
    }
}
