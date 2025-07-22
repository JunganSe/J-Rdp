namespace Core.Profiles;

/// <summary>
/// Used to represent a profile when communicating with the UI.
/// </summary>
public class ProfileInfo
{
    public int Id { get; set; }       = -1;
    public bool Enabled { get; set; } = false;
    public string Name { get; set; }  = "";
}
