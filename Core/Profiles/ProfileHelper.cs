namespace Core.Profiles;

internal static class ProfileHelper
{
    public static List<Profile> GetDeepCopies(List<Profile> profiles, bool copyId = true) =>
        profiles.Select(p => GetDeepCopy(p, copyId)).ToList();

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
            profileCopy.Id = profile.Id;

        return profileCopy;
    }

    public static List<ProfileInfo> GetProfileInfos(List<Profile> profiles) =>
        profiles.Select(profile => new ProfileInfo()
            {
                Id = profile.Id,
                Enabled = profile.Enabled,
                Name = profile.Name
            })
            .ToList();

    /// <summary>
    /// Sets the enabled states of profiles based on the enabled states of profileInfos, where their Id is matching.
    /// </summary>
    public static void SetEnabledStatesFromMatchingProfileInfos(List<Profile> profiles, List<ProfileInfo> profileInfos)
    {
        foreach (var profile in profiles)
        {
            var profileInfo = profileInfos.FirstOrDefault(pi => pi.Id == profile.Id);
            profile.Enabled = profileInfo?.Enabled ?? false;
        }
    }
}
