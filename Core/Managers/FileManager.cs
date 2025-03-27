using Core.Helpers;
using Core.Models;
using NLog;

namespace Core.Managers;

internal class FileManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly RdpFileManager _rdpManager = new();
    private readonly List<string> _processedFilePaths = [];

    public void SetDeleteDelay(int deleteDelay)
    {
        if (deleteDelay == _rdpManager.DeleteDelay)
            return;

        _rdpManager.DeleteDelay = deleteDelay;
        _logger.Info($"Delete delay set to {_rdpManager.DeleteDelay} ms.");
    }

    public void ProcessProfileWrappers(IEnumerable<ProfileWrapper> profileWrappers)
    {
        _processedFilePaths.Clear();
        var processableProfileWrappers = profileWrappers.Where(pi => pi.DirectoryExists);
        foreach (var profileWrapper in processableProfileWrappers)
            ProcessNewFiles(profileWrapper);
    }

    private void ProcessNewFiles(ProfileWrapper profileWrapper)
    {
        var newFiles = profileWrapper.NewFiles.Where(file => !_processedFilePaths.Contains(file.FullName));

        if (!newFiles.Any())
            return;

        LogNewFiles(profileWrapper.Profile, newFiles);

        foreach (var newFile in newFiles)
            ProcessFileOnFilterMatch(newFile, profileWrapper.Profile);
    }

    private void LogNewFiles(Profile profile, IEnumerable<FileInfo> newFiles)
    {
        string s = newFiles.Count() > 1 ? "s" : "";
        string fileNames = string.Join("", newFiles.Select(f => $"\n  {f.Name}"));
        _logger.Debug($"Profile '{profile.Name}' found {newFiles.Count()} new file{s} in '{profile.WatchFolder}': {fileNames}");
    }

    private void ProcessFileOnFilterMatch(FileInfo file, Profile profile)
    {
        if (!FileHelper.FileNameMatchesFilter(file.Name, profile.Filter))
            return;

        _logger.Info($"Profile '{profile.Name}' found a match on '{file.FullName}' using filter '{profile.Filter}'.");

        _processedFilePaths.Add(file.FullName);
        _rdpManager.ProcessFile(file, profile);
    }
}
