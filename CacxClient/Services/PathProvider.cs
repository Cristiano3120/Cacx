using CacxClient.Abstractions;
using System.Diagnostics;
using System.IO;

namespace CacxClient.Services;

internal class PathProvider : IPathProvider
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
}
