using System.Text.Json.Serialization;

namespace Core.Profiles;

internal class Profile
{
    private const int _defaultId = -1;

    private int _id = _defaultId;
    private string _name = ProfileConstants.DefaultName;

    [JsonIgnore]
    public int Id
    {
        get => _id;
        set => _id = (_id == _defaultId) ? value : throw new InvalidOperationException("Id is already set.");
    }
    public string Name
    {
        get => _name;
        init => _name = !string.IsNullOrWhiteSpace(value) ? value : ProfileConstants.DefaultName;
    }
    public bool Enabled { get; set; }           = true;
    public string WatchFolder { get; init; }    = "";
    public string Filter { get; set; }          = "";
    public string MoveToFolder { get; init; }   = "";
    public bool Launch { get; init; }           = false;
    public bool Delete { get; init; }           = false;
    public List<string> Settings { get; init; } = [];

    public Profile()
    {
    }

    public Profile(int id)
    {
        Id = id;
    }
}
