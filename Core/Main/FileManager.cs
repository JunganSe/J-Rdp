using Core.Configuration;
using NLog;
using System.Diagnostics;

namespace Core.Main;

internal class FileManager
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public void ProcessFile(FileInfo file, Config config)
    {
        Move(file, config);
        Edit(file, config.Settings);
        Launch(file);
        Delete(file);
    }



    private void Move(FileInfo file, Config config)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(config.MoveToFolder))
                return;

            bool isPathAbsolute = Path.IsPathFullyQualified(config.MoveToFolder); // e.g. 'C:\Foo\Bar'
            if (!isPathAbsolute)
            {
                _logger.Warn($"Can not move file '{file.Name}', target path is not absolute: {config.MoveToFolder}");
                return;
            }

            Directory.CreateDirectory(config.MoveToFolder);
            string fullTargetPath = Path.Combine(config.MoveToFolder, file.Name);
            file.MoveTo(fullTargetPath, overwrite: true);

            _logger.Trace($"Moved file '{file.Name}' from '{config.WatchFolder}' to '{config.MoveToFolder}'.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to move file.");
        }
    }

    private void Edit(FileInfo file, Dictionary<string, string> settings)
    {

    }

    private void Launch(FileInfo file)
    {
        try
        {
            file.Refresh();
            var process = new ProcessStartInfo(file.FullName) { UseShellExecute = true };
            Process.Start(process);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to launch file.");
        }
    }

    private void Delete(FileInfo file)
    {

    }
}
