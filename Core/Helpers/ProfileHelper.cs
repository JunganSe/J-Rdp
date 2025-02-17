using Core.Models;

namespace Core.Helpers;

internal static class ProfileHelper
{
    /// <summary> Creates a deep copy of a profile. Id is not copied. </summary>
    public static Profile DeepCopy(Profile profile) => new()
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
}
