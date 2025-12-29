namespace CacxShared.Abstractions;

public interface IPathProvider
{
    /// <summary>
    /// relativePath has to include the extension.
    /// </summary>
    /// <param name="relativePath">has to include the extension of the file.</param>
    public string GetPath(string relativePath);

    public string GetAppDataPath(string? relativePath);
}
