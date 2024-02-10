using Core.Helpers;

namespace Core.Configuration;

internal class ConfigInfo
{
    private readonly FileInfoFullNameEqualityComparer _fileComparer = new FileInfoFullNameEqualityComparer();

    public Config Config { get; }
    //public DirectoryInfo Directory { get; }
    public IEnumerable<FileInfo> Files { get; private set; }
    public IEnumerable<FileInfo> LastFiles { get; private set; }
    public IEnumerable<FileInfo> NewFiles => Files.Except(LastFiles, _fileComparer);
    public bool DirectoryExists => Directory.Exists(Config.WatchFolder);

    public ConfigInfo(Config config)
    {
        Config = config;
        //Directory = new DirectoryInfo(config.WatchFolder);
        Files = [];
        LastFiles = [];
    }

    public void UpdateFiles()
    {
        LastFiles = Files;

        if (DirectoryExists)
            Files = Directory.GetFiles(Config.WatchFolder).Select(s => new FileInfo(s));
        else
            Files = [];
    }
}
