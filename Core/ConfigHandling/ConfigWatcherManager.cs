using Core.FileHandling;

namespace Core.ConfigHandling;

internal class ConfigWatcherManager
{
    private ConfigWatcher? _configWatcher;

    public void StartConfigWatcher(Action callback)
    {
        string directory = FileHelper.GetConfigDirectory();
        string fileName = ConfigConstants.FileName;
        _configWatcher = new ConfigWatcher(directory, fileName, callback);
    }

    public void StopAndDisposeConfigWatcher()
    {
        try
        {
            if (_configWatcher == null)
                return;

            _configWatcher.EnableRaisingEvents = false;
            _configWatcher.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Swallow exception if it's already disposed.
        }
    }
}
