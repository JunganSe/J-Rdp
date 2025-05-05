using Core.Configs;
using Core.Profiles;
using NLog;

namespace Core.Files;

internal class RdpFileManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly RdpFileWorker _rdpFileWorker = new();

    public int DeleteDelay { get; set; } = ConfigConstants.DeleteDelay_Default;

    public void ProcessFile(FileInfo file, Profile profile)
    {
        if (!string.IsNullOrWhiteSpace(profile.MoveToFolder))
            Move(file, profile.MoveToFolder);

        if (profile.Settings.Count > 0)
            Edit(file, profile.Settings);

        if (profile.Launch)
            Launch(file);

        if (profile.Delete)
        {
            if (profile.Launch)
                Thread.Sleep(DeleteDelay); // To allow the file time to be opened/released before deletion.
            Delete(file, recycle: true);
        }
    }



    private void Move(FileInfo file, string moveToFolder)
    {
        string sourceDirectory = file.DirectoryName ?? "(unknown)";
        try
        {
            _rdpFileWorker.Move(file, moveToFolder);
            _logger.Info($"Moved file '{file.Name}' from '{sourceDirectory}' to '{moveToFolder}'.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to move file '{file.Name}' from '{sourceDirectory}' to '{moveToFolder}'.");
        }
    }

    private void Edit(FileInfo file, List<string> settings)
    {
        try
        {
            _rdpFileWorker.Edit(file, settings);
            string s = settings.Count > 1 ? "s" : "";
            _logger.Info($"Applied {settings.Count} setting{s} to file '{file.Name}' in '{file.DirectoryName}'.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to edit file '{file.Name}' in '{file.DirectoryName}'.");
        }
    }

    private void Launch(FileInfo file)
    {
        try
        {
            _rdpFileWorker.Launch(file);
            _logger.Info($"Launched file '{file.Name}' in '{file.DirectoryName}'.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to launch file '{file.Name}' in '{file.DirectoryName}'.");
        }
    }

    private void Delete(FileInfo file, bool recycle = true)
    {
        try
        {
            _rdpFileWorker.Delete(file, recycle);
            string verb = recycle ? "Recycled" : "Permanently deleted";
            _logger.Info($"{verb} file '{file.Name}' in '{file.DirectoryName}'.");
        }
        catch (Exception ex)
        {
            string recycleOrDelete = recycle ? "recycle" : "permanently delete";
            _logger.Error(ex, $"Failed to {recycleOrDelete} file '{file.Name}' in '{file.DirectoryName}'.");
        }
    }
}
