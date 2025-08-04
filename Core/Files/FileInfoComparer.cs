namespace Core.Files;

internal record EqualityComparer_FileInfo_FullName : IEqualityComparer<FileInfo>
{
    public bool Equals(FileInfo? a, FileInfo? b) =>
        a?.FullName == b?.FullName;

    public int GetHashCode(FileInfo? obj) =>
        obj?.FullName?.GetHashCode() ?? 0;
}
