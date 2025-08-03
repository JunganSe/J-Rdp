namespace Core.Profiles;

internal class EqualityComparer_Profile_AllExceptId : IEqualityComparer<Profile>
{
    public bool Equals(Profile? a, Profile? b)
    {
        throw new NotImplementedException();
    }

    public int GetHashCode(Profile obj)
    {
        throw new NotImplementedException();
    }
}
