using Core.Helpers;

namespace Core.Configuration;

internal class ConfigInfo
{
    private readonly FileInfoEqualityComparer_FullName _fileComparer = new();

    public Config Config { get; }
    public IEnumerable<FileInfo> Files { get; private set; }
    public IEnumerable<FileInfo> LastFiles { get; private set; }
    public IEnumerable<FileInfo> NewFiles => Files.Except(LastFiles, _fileComparer);
    public bool DirectoryExists => Directory.Exists(Config.WatchFolder);
    public string DirectoryFullPath => Path.GetFullPath(Config.WatchFolder);

    public ConfigInfo(Config config)
    {
        Config = config;
        Files = [];
        LastFiles = [];
    }

    public void UpdateFiles()
    {
        LastFiles = Files;
        Files = (DirectoryExists) ? GetFiles() : [];
    }



    private IEnumerable<FileInfo> GetFiles()
        => Directory.GetFiles(Config.WatchFolder).Select(path => new FileInfo(path));
}
