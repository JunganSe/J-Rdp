using Core.Profiles;

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

    public void Stop()
    {
        _controller?.Stop();
    }

    /// <summary>
    /// Tell the core controller which method should be called after the config (in memory) has been updated.
    /// </summary>
    public void SetCallback_ConfigUpdated(ProfileHandler callback)
    {
        _controller?.SetCallback_ConfigUpdated(callback);
    }

    /// <summary>
    /// Tell the core controller to open the config file in the default editor.
    /// </summary>
    public void OpenConfigFile()
    {
        _controller?.OpenConfigFile();
    }

    /// <summary>
    /// Tell the core controller to update the profiles enabled state (in config file and memory).
    /// </summary>
    public void UpdateProfilesEnabledState(List<ProfileInfo> profileInfos)
    {
        _controller?.UpdateProfilesEnabledState(profileInfos);
    }
}
