using Core.Models;

namespace Core.Helpers;

internal static class ProfileHelper
{
    /// <summary> Creates a deep copy of a profile. </summary>
    public static Profile GetDeepCopy(Profile profile, bool copyId = true)
    {
        var profileCopy = new Profile()
        {
            Enabled = profile.Enabled,
            Name = profile.Name,
            WatchFolder = profile.WatchFolder,
            Filter = profile.Filter,
            MoveToFolder = profile.MoveToFolder,
            Launch = profile.Launch,
            Delete = profile.Delete,
            Settings = [.. profile.Settings]
        };
        if (copyId)
            profileCopy.SetId(profile.Id);
        return profileCopy;
    }
}
