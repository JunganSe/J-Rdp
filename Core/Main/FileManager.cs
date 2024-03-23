using Core.Configuration;
using NLog;
using System.Diagnostics;

namespace Core.Main;

internal class FileManager
{
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
            Delete(file);
    }



    private void Move(FileInfo file, string targetDirectory)
    {
        try
        {
            bool isPathAbsolute = Path.IsPathFullyQualified(targetDirectory); // e.g. 'C:\Foo\Bar'
            if (!isPathAbsolute)
            {
                _logger.Warn($"Can not move file '{file.Name}', target path is not absolute: {targetDirectory}");
                return;
            }

            string sourceDirectory = file.DirectoryName ?? "(unknown)";
            Directory.CreateDirectory(targetDirectory);
            string fullTargetPath = Path.Combine(targetDirectory, file.Name);
            file.MoveTo(fullTargetPath, overwrite: true);

            _logger.Info($"Moved file '{file.Name}' from '{sourceDirectory}' to '{targetDirectory}'.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to move file '{file.Name}' to '{targetDirectory}'.");
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

            _logger.Info($"Appended {settings.Count} lines to file '{file.Name}'.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to edit file '{file.Name}'.");
        }
    }

    private void Launch(FileInfo file)
    {
        try
        {
            file.Refresh();
            var process = new ProcessStartInfo(file.FullName) { UseShellExecute = true };
            Process.Start(process);
            _logger.Info($"Launched file '{file.Name}'.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to launch file '{file.Name}'.");
        }
    }

    private void Delete(FileInfo file)
    {
        // TODO: Implement deletion of file.
    }
}
