namespace Core.Configuration;

internal class ConfigInfo
{
    public Config Config { get; }
    public DirectoryInfo Directory { get; }
    public IEnumerable<FileInfo> Files { get; private set; }

    public ConfigInfo(Config config)
    {
        Config = config;
        Directory = new DirectoryInfo(config.WatchFolder);
        Files = [];
    }

    public void UpdateFiles()
    {
        if (Directory.Exists)
            Files = Directory.GetFiles();
        else
            Files = [];
    }
}
