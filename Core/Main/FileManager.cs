using Core.Configuration;
using Microsoft.VisualBasic.FileIO;
using NLog;
using System.Diagnostics;

namespace Core.Main;

internal class FileManager
{
    private const int _deleteDelay = 1000;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ProcessFile(FileInfo file, Config config)
    {
        if (!string.IsNullOrWhiteSpace(config.MoveToFolder))
            Move(file, config.MoveToFolder);

        if (config.Settings.Count > 0)
            Edit(file, config.Settings);

        if (config.Launch)
            Launch(file);

        if (config.Delete)
            Delete(file, recycle: true);
    }



    private void Move(FileInfo file, string targetDirectory)
    {
        string sourceDirectory = file.DirectoryName ?? "(unknown)";
        try
        {
            bool isPathAbsolute = Path.IsPathFullyQualified(targetDirectory); // e.g. 'C:\Foo\Bar'
            if (!isPathAbsolute)
            {
                _logger.Warn($"Can not move file '{file.Name}', target path is not absolute: {targetDirectory}");
                return;
            }

            Directory.CreateDirectory(targetDirectory);
            string fullTargetPath = Path.Combine(targetDirectory, file.Name);
            file.MoveTo(fullTargetPath, overwrite: true);

            _logger.Info($"Moved file '{file.Name}' from '{sourceDirectory}' to '{targetDirectory}'.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to move file '{file.Name}' from '{sourceDirectory}' to '{targetDirectory}'.");
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

            _logger.Info($"Appended {settings.Count} lines to file '{file.Name}' in '{file.DirectoryName}'.");
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
            Thread.Sleep(_deleteDelay);

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
