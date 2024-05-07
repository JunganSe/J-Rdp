using Core.Configuration;
using Core.Constants;
using Core.Helpers;

namespace Core.Workers;

internal class ConfigWatcherWorker
{
    private ConfigWatcher? _configWatcher;

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

    public void StartConfigWatcher(Action callback)
    {
        string directory = FileHelper.GetConfigDirectory();
        string fileName = ConfigConstants.FileName;
        _configWatcher = new ConfigWatcher(directory, fileName, callback: callback);
    }
}
