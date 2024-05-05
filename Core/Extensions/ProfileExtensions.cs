using Core.Configuration;
using Core.Helpers;

namespace Core.Extensions;

internal static class ProfileExtensions
{
    public static void RemoveDisabledProfiles(this IEnumerable<Profile> profiles) 
        => profiles = profiles.Where(p => p.Enabled);

    public static void RemoveInvalidProfiles(this IEnumerable<Profile> profiles) 
        => profiles = profiles.Where(p => p.IsValid(out _));

    public static bool IsValid(this Profile profile, out string reason) 
        => ProfileHelper.IsProfileValid(profile, out reason);
}
