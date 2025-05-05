using Core.Files;

namespace Core.Profiles;

internal class ProfileWrapper
{
    private readonly EqualityComparer_FileInfo_FullName _fileComparer = new();

    public Profile Profile { get; }
    public List<FileInfo> Files { get; private set; }
    public List<FileInfo> LastFiles { get; private set; }
    public List<FileInfo> NewFiles => Files.Except(LastFiles, _fileComparer).ToList();
    public bool DirectoryExists => Directory.Exists(Profile.WatchFolder);
    public string DirectoryFullPath => Path.GetFullPath(Profile.WatchFolder);

    public ProfileWrapper(Profile profile)
    {
        Profile = profile;
        Files = [];
        LastFiles = [];
    }

    public void UpdateFiles()
    {
        LastFiles = Files;
        Files = DirectoryExists ? GetFiles() : [];
    }



    private List<FileInfo> GetFiles() =>
        Directory.GetFiles(Profile.WatchFolder).Select(path => new FileInfo(path)).ToList();
}
