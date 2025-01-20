using System.Reflection;

namespace Auxiliary;

public static class AssemblyHelper
{
    public static string GetAssemblyName(Type type) =>
        GetAssemblyName(type.Assembly);

    public static string GetAssemblyName(Assembly assembly) =>
        assembly.GetName().Name ?? "";



    public static string GetAssemblyVersion(Type type) =>
        GetAssemblyVersion(type.Assembly);

    public static string GetAssemblyVersion(Assembly assembly)
    {
        string longVersion = assembly.GetName().Version?.ToString() ?? "0.0.0.0";
        int index = longVersion.LastIndexOf('.');
        return (index > 0) ? longVersion[..index] : "";
    }
}
