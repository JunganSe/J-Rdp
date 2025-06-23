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
        if (show)
            OpenConsole();
        else
            _worker.CloseConsole();
    }

    private void OpenConsole()
    {
        try
        {
            _logger.Trace("Opening log console...");
            bool openConsoleIsSuccess = _worker.TryAllocateConsole();
            _worker.SetConsoleTitle();
            _worker.PrintInfoMessage();
            _worker.DisableConsoleCloseButton();
            _worker.SetEvent_CloseConsoleOnCommand();

            if (openConsoleIsSuccess)
                _logger.Info("Opened log console.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error opening log console.");
        }
    }
}
