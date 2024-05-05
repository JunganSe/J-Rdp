using Core.Configuration;
using Core.Constants;
using Core.Helpers;

namespace Core.Extensions;

internal static class ProfileExtensions
{
    public static void RemoveDisabled(this List<Profile> profiles) 
        => profiles.RemoveAll(p => !p.Enabled);

    public static void RemoveInvalid(this List<Profile> profiles) 
        => profiles.RemoveAll(p => !p.IsValid(out _));

    public static bool IsValid(this Profile profile, out string reason) 
        => ProfileHelper.IsProfileValid(profile, out reason);

    public static void AddDefaultFilterFileEndings(this List<Profile> profiles)
        => profiles.ForEach(p => p.AddDefaultFilterFileEnding());

    public static void AddDefaultFilterFileEnding(this Profile profile)
    {
        string defaultFileEnding = ConfigConstants.Profile_DefaultFilterFileEnding;
        bool hasFileEnding = profile.Filter.EndsWith(defaultFileEnding, StringComparison.OrdinalIgnoreCase);
        if (!hasFileEnding)
            profile.Filter += defaultFileEnding;
    }
}
