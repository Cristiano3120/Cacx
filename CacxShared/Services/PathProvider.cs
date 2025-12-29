using CacxShared.Abstractions;
using System.Diagnostics;

namespace CacxShared.Services;

public sealed class PathProvider : IPathProvider
{
    public string GetPath(string relativePath)
    {
        string path = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (!Debugger.IsAttached)
        {
            // .exe is running
            return path;
        }

        int indexOfBin = path.IndexOf(@"\bin", StringComparison.OrdinalIgnoreCase);
        if (indexOfBin >= 0)
        {
            path = Path.Combine(path[..indexOfBin], relativePath);
        }

        return path;
    }

    /// <summary>
    /// Gets the full path to the application's data directory, optionally appending a specified relative path.
    /// </summary>
    /// <remarks>If the application's data directory does not exist, it is created automatically. The base
    /// directory is located under the current user's ApplicationData folder.</remarks>
    /// <param name="relativePath">A relative path to append to the application's data directory. If null, only the base application data directory
    /// path is returned.</param>
    /// <returns>The full path to the application's data directory, or the combined path if a relative path is specified.</returns>
    public string GetAppDataPath(string? relativePath)
    {
        string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appPath = Path.Combine(basePath, "Cacx");

        _ = Directory.CreateDirectory(appPath);

        return relativePath is null 
            ? appPath 
            : Path.Combine(appPath, relativePath);
    }
}
