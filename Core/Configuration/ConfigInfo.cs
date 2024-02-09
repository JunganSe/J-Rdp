namespace Core.Configuration;

internal class ConfigInfo
{
    public Config Config { get; }
    public DirectoryInfo Directory { get; }
    public IEnumerable<FileInfo> Files { get; private set; }
    public IEnumerable<FileInfo> LastFiles { get; private set; }
    public IEnumerable<FileInfo> NewFiles => Files.Except(LastFiles);

    public ConfigInfo(Config config)
    {
        Config = config;
        Directory = new DirectoryInfo(config.WatchFolder);
        Files = [];
        LastFiles = [];
    }

    public void UpdateFiles()
    {
        LastFiles = Files;

        if (Directory.Exists)
            Files = Directory.GetFiles();
        else
            Files = [];
    }
}
