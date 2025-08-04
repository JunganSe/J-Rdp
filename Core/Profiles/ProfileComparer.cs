namespace Core.Profiles;

internal class EqualityComparer_Profile_AllExceptId : IEqualityComparer<Profile>
{
    public bool Equals(Profile? a, Profile? b)
    {
        if (a is null && b is null)
            return true;
        if (a is null || b is null)
            return false;

        return a.Name == b.Name
            && a.Enabled == b.Enabled
            && a.WatchFolder.Equals(b.WatchFolder, StringComparison.OrdinalIgnoreCase)
            && a.Filter == b.Filter
            && a.MoveToFolder.Equals(b.MoveToFolder, StringComparison.OrdinalIgnoreCase)
            && a.Launch == b.Launch
            && a.Delete == b.Delete
            && a.Settings.Order().SequenceEqual(b.Settings.Order());
    }

    public int GetHashCode(Profile obj)
    {
        if (obj is null)
            return 0;

        var hash = new HashCode();
        hash.Add(obj.Name);
        hash.Add(obj.Enabled);
        hash.Add(obj.WatchFolder, StringComparer.OrdinalIgnoreCase);
        hash.Add(obj.Filter);
        hash.Add(obj.MoveToFolder, StringComparer.OrdinalIgnoreCase);
        hash.Add(obj.Launch);
        hash.Add(obj.Delete);

        // Add settings in a order-independent way
        foreach (string setting in obj.Settings.Order())
            hash.Add(setting);

        return hash.ToHashCode();
    }
}
