using Core.Files;

namespace Core.Profiles;

internal class ProfileWrapper
{
    private readonly EqualityComparer_FileInfo_FullName _fileComparer = new();

    public Profile Profile { get; }
    public IEnumerable<FileInfo> Files { get; private set; }
    public IEnumerable<FileInfo> LastFiles { get; private set; }
    public IEnumerable<FileInfo> NewFiles => Files.Except(LastFiles, _fileComparer);
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



    private IEnumerable<FileInfo> GetFiles() =>
        Directory.GetFiles(Profile.WatchFolder).Select(path => new FileInfo(path));
}
