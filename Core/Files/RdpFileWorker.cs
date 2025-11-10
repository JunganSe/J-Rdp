using Auxiliary;
using Microsoft.VisualBasic.FileIO;

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
                throw new ArgumentException($"Invalid setting: '${setting}'. No semicolon present.");
            string key = setting[..lastColonIndex];
            fileLines.RemoveAll(line => line.Contains(key));
        }
        fileLines.Add("");
        fileLines.AddRange(settings);

        File.WriteAllLines(file.FullName, fileLines);
    }

    public void Launch(FileInfo file)
    {
        file.Refresh();
        FileSystemOpener.OpenFile(file.FullName);
    }

    public void Delete(FileInfo file, bool recycle = true)
    {
        var recycleOption = recycle
            ? RecycleOption.SendToRecycleBin
            : RecycleOption.DeletePermanently;
        file.Refresh();
        FileSystem.DeleteFile(file.FullName, UIOption.OnlyErrorDialogs, recycleOption);
    }
}
