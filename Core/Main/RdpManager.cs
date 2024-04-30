using Core.Configuration;
using Core.Constants;
using Core.Helpers;
using Microsoft.VisualBasic.FileIO;
using NLog;
using System.Diagnostics;

namespace Core.Main;

internal class RdpManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public int DeleteDelay { get; set; } = ConfigConstants.DeleteDelay_Default;

    public void ProcessFile(FileInfo file, Profile profile)
    {
        if (!string.IsNullOrWhiteSpace(profile.MoveToFolder))
            Move(file, profile.MoveToFolder);

        if (profile.Settings.Count > 0)
            Edit2(file, profile.Settings);

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
            if (!FileHelper.IsPathAbsolute(sourceDirectory))
                throw new ArgumentException($"Bad file path: '{sourceDirectory}'");

            string targetDirectory = Path.GetFullPath(Path.Combine(sourceDirectory, moveToFolder));
            string fullTargetPath = Path.Combine(targetDirectory, file.Name);
            Directory.CreateDirectory(targetDirectory);
            file.MoveTo(fullTargetPath, overwrite: true);

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
            if (settings.Count == 0)
                return;

            using var streamWriter = new StreamWriter(file.FullName, append: true);

            streamWriter.WriteLine();
            foreach (string setting in settings)
                streamWriter.WriteLine(setting);

            string linesWord = (settings.Count == 1) ? "line" : "lines";
            _logger.Info($"Appended {settings.Count} {linesWord} to file '{file.Name}' in '{file.DirectoryName}'.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to edit file '{file.Name}' in '{file.DirectoryName}'.");
        }
    }

    private void Edit2(FileInfo file, List<string> settings)
    {
        try
        {
            if (settings.Count == 0)
                return;

            var fileLines = File.ReadAllLines(file.FullName).ToList();

            foreach (string setting in settings)
            {
                int lastColonIndex = setting.LastIndexOf(':');
                var key = setting[..lastColonIndex];
                fileLines.RemoveAll(l => l.Contains(key));
            }

            fileLines.Add("");
            fileLines.AddRange(settings);

            File.WriteAllLines(file.FullName, fileLines);
                        
            string s = (settings.Count > 1) ? "s" : "";
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
            file.Refresh();
            var process = new ProcessStartInfo(file.FullName) { UseShellExecute = true };
            Process.Start(process);
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
            file.Refresh();
            var recycleOption = (recycle) ? RecycleOption.SendToRecycleBin : RecycleOption.DeletePermanently;
            FileSystem.DeleteFile(file.FullName, UIOption.OnlyErrorDialogs, recycleOption);

            string verb = (recycle) ? "Recycled" : "Permanently deleted";
            _logger.Info($"{verb} file '{file.Name}' in '{file.DirectoryName}'.");
        }
        catch (Exception ex)
        {
            string verb = (recycle) ? "recycle" : "permanently delete";
            _logger.Error(ex, $"Failed to {verb} file '{file.Name}' in '{file.DirectoryName}'.");
        }
    }
}
