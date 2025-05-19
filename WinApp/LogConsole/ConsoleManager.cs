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
        _worker.AllocateConsole();
        _worker.SetConsoleTitle();
        _worker.DisableConsoleCloseButton();
        _worker.RedirectConsoleOutput();
        _worker.SetControlHandler();
        _worker.PrintInfoMessage();
        _logger.Info("Opened log console.");
    }
}
