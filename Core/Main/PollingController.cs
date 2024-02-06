using NLog;

namespace Core.Main;

public class PollingController
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly int _pollingInterval = 1000;

    public PollingController()
    {
    }

    public void Start()
    {
        while (true)
        {
            MainLoop();
            Thread.Sleep(_pollingInterval);
        }
    }

    private void MainLoop()
    {
        // TODO: Implement all this:
        // - Read config file.
        // - Check if config file has changed (compare with previous version)
        //   - Update configs if needed.
        // - Loop through configs.
        //   - Check if the folder to watch exists.
        //   - If folder exists, list all files in it.
        //   - Ignore files that are already handled. (How?)
        //   - Loop though files and check for filter match.
        //   - If file matches filter:
        //     - Register the file as handled. (How?)
        //     - Run instructions on the file according to config.
    }
}
