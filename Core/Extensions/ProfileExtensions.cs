using Core.Configuration;
using Core.Helpers;

namespace Core.Extensions;

internal static class ProfileExtensions
{
    public static void RemoveDisabledProfiles(this IEnumerable<Profile> profiles) 
        => profiles = profiles.Where(p => p.Enabled);

    public static void RemoveInvalidProfiles(this IEnumerable<Profile> profiles)
    {
        profiles = profiles.Where(p => ProfileHelper.IsProfileValid(p, out _));
    }
}
