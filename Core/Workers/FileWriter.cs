namespace Core.Workers;

internal class FileWriter
{
    public void WriteFile(string path, string content)
    {
        throw new NotImplementedException(); // TODO: Implement.

        // v1
        using var writer = new StreamWriter(path, false);
        writer.Write(content);

        // v2
        File.WriteAllText(path, content);
    }
}
