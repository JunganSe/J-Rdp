using Core.Models;

namespace WinApp.Managers;

internal class CoreManager
{
    private Core.Controller? _controller;

    public void Initialize()
    {
        _controller = new();
        //_controller.SetCallback_ConfigUpdated(dummy);
    }

    public void Run()
    {
        _controller?.Run();
    }

    public void UpdateProfilesEnabledState(List<ProfileInfo> profileInfos)
    {
        _controller?.UpdateProfilesEnabledState(profileInfos);
    }
}
