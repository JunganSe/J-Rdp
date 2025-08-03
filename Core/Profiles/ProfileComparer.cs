namespace Core.Profiles;

internal class EqualityComparer_Profile_AllExceptId : IEqualityComparer<Profile>
{
    public record ProfileKey(
        string Name,
        bool Enabled,
        string WatchFolder,
        string Filter,
        string MoveToFolder,
        bool Launch,
        bool Delete,
        List<string> Settings);

    private static ProfileKey GetKey(Profile profile) => new(
        profile.Name,
        profile.Enabled,
        profile.WatchFolder,
        profile.Filter,
        profile.MoveToFolder,
        profile.Launch,
        profile.Delete,
        profile.Settings);


    public bool Equals(Profile? a, Profile? b)
    {
        if (a is null && b is null)
            return true;
        if (a is null || b is null)
            return false;

        var keyA = GetKey(a);
        var keyB = GetKey(b);
        return EqualityComparer<ProfileKey>.Default.Equals(keyA, keyB);
    }

    public int GetHashCode(Profile obj)
    {
        return EqualityComparer<ProfileKey>.Default.GetHashCode(GetKey(obj));
    }
}
