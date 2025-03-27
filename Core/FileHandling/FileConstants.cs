namespace Core.FileHandling;

internal static class FileConstants
{
    public const int ReadFile_TryCountMax = 3; // Must be >= 1
    public const int ReadFile_RetryDelay = 200; // ms
    public const int FileChanged_DebounceDelay = 50; // ms
}
