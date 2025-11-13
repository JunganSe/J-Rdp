namespace Core.LogDisplay;

public interface ILogDisplayManager
{
    public void SetCallback_LogClosed(Action callback);
    public void SetVisibility(bool show);
}
