namespace Core.Extensions;

internal static class CommonExtensions
{
    public static string NormalizePath(this string path)
        => path.Replace("/", "\\");
}
