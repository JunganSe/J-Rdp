using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;

namespace Core.Files;

internal class RdpFileWorker
{
    public void Move(FileInfo file, string moveToFolder)
    {
        string sourceDirectory = file.DirectoryName ?? "(unknown)";
        string targetDirectory = Path.GetFullPath(Path.Combine(sourceDirectory, moveToFolder));
        string fullTargetPath = Path.Combine(targetDirectory, file.Name);

        Directory.CreateDirectory(targetDirectory);
        file.MoveTo(fullTargetPath, overwrite: true);
    }

    public void Edit(FileInfo file, List<string> settings)
    {
        var fileLines = File.ReadAllLines(file.FullName).ToList();

        foreach (string setting in settings)
        {
            int lastColonIndex = setting.LastIndexOf(':');
            if (lastColonIndex == -1)
                throw new ArgumentException("Invalid setting, no semicolon present.");
            var key = setting[..lastColonIndex];
            fileLines.RemoveAll(l => l.Contains(key));
        }
        fileLines.Add("");
        fileLines.AddRange(settings);

        File.WriteAllLines(file.FullName, fileLines);
    }

    public void Launch(FileInfo file)
    {
        file.Refresh();
        var process = new ProcessStartInfo(file.FullName) { UseShellExecute = true };
        Process.Start(process);
    }

    public void Delete(FileInfo file, bool recycle = true)
    {
        file.Refresh();
        var recycleOption = recycle ? RecycleOption.SendToRecycleBin : RecycleOption.DeletePermanently;
        FileSystem.DeleteFile(file.FullName, UIOption.OnlyErrorDialogs, recycleOption);
    }
}
