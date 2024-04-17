﻿using Core.Configuration;
using Core.Constants;
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



    private void Move(FileInfo file, string targetDirectory)
    {
        string sourceDirectory = file.DirectoryName ?? "(unknown)";
        try
        {
            Directory.CreateDirectory(targetDirectory);
            string fullTargetPath = Path.Combine(targetDirectory, file.Name);
            file.MoveTo(fullTargetPath, overwrite: true);

            _logger.Info($"Moved file '{file.Name}' from '{sourceDirectory}' to '{fullTargetPath}'.");
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

            string linesWord = (settings.Count == 1) ? "line" : "lines";
            _logger.Info($"Appended {settings.Count} {linesWord} to file '{file.Name}' in '{file.DirectoryName}'.");
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
