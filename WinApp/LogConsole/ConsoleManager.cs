namespace WinApp.LogConsole;

/// <summary> Windows exclusive manager for opening and closing a console log window. </summary>
internal class ConsoleManager
{
    private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly ConsoleWorker _worker = new();

    public void SetCallback_ConsoleClosed(Action callback) =>
        _worker.SetCallback_ConsoleClosed(callback);

    public void SetVisibility(bool show)
    {
        bool isConsoleOpen = _worker.IsConsoleWindowOpen();

        if (show && !isConsoleOpen)
            OpenConsole();
        else if (!show && isConsoleOpen)
            _worker.CloseConsole();
    }

    private void OpenConsole()
    {
        try
        {
            _logger.Trace("Opening log console...");
            bool isConsoleOpened = _worker.TryAllocateConsole();
            _worker.SetConsoleTitle();
            _worker.PrintInfoMessage();
            _worker.DisableConsoleCloseButton();
            _worker.SetEvent_CloseConsoleOnCommand();

            if (isConsoleOpened)
                _logger.Info("Opened log console.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error opening log console.");
        }
    }
}
