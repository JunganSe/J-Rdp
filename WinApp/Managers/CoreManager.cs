using Core.Delegates;
using Core.Models;

namespace WinApp.Managers;

internal class CoreManager
{
    private Core.Controller? _controller;

    public void Initialize()
    {
        _controller = new();
    }

    public void Run()
    {
        _controller?.Run();
    }

    public void SetCallback_ConfigUpdated(ProfileHandler callback)
    {
        _controller?.SetCallback_ConfigUpdated(callback);
    }

    public void UpdateProfilesEnabledState(List<ProfileInfo> profileInfos)
    {
        _controller?.UpdateProfilesEnabledState(profileInfos);
    }
}
