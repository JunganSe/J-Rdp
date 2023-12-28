using Core.Interfaces;

namespace Core.Components;

public class Controller
{
    private readonly IUi _ui;

    public Controller(IUi ui)
    {
        _ui = ui;
    }



    public void Start()
    {
        throw new NotImplementedException();
    }
}
