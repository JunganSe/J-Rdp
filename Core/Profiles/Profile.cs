using Core.Configs;
using System.Text.Json.Serialization;

namespace Core.Profiles;

internal class Profile
{
    private const int _defaultId = -1;
    private string _name = ConfigConstants.Profile_DefaultName;

    [JsonIgnore]
    public int Id { get; private set; }         = _defaultId;
    public bool Enabled { get; set; }           = true;
    public string Name
    {
        get => _name;
        init => _name = !string.IsNullOrWhiteSpace(value) ? value : ConfigConstants.Profile_DefaultName;
    }
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

    public void SetId(int id)
    {
        if (Id != _defaultId)
            throw new InvalidOperationException("Id is already set.");
        
        Id = id;
    }
}
