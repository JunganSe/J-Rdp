namespace Core.Workers;

internal class FileWriter
{
    public void WriteFile(string path, string content)
    {
        throw new NotImplementedException();

        // v1
        using var writer = new StreamWriter(path, false);
        writer.Write(content);

        // v2
        File.WriteAllText(path, content);
    }
}
