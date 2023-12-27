using Core.Interfaces;

namespace Core.Components;

public class Watcher
{
    private readonly IUi _ui;

    public Watcher(IUi ui)
    {
        _ui = ui;
    }



    public void Start()
    {
        throw new NotImplementedException();
    }



    private Config GetConfiguration()
    {
        throw new NotImplementedException();
    }
}
