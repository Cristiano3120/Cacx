using CacxServer.Interfaces;

namespace CacxServer.Helper;

public enum PathType
{
    Logs,
}

public class PathHelper(IConfiguration configuration) : IPathHelper
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly string _baseDir = AppContext.BaseDirectory;

    public string GetPath(PathType type)
    {
        string relativePath = type switch
        {
            PathType.Logs => _configuration[$"Paths:{PathType.Logs}"] ?? "logs",
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        string? envVar = Environment.GetEnvironmentVariable(variable: $"{type.ToString().ToUpperInvariant()}_DIR");
        string finalPath = !string.IsNullOrWhiteSpace(envVar) 
            ? envVar 
            : Path.Combine(_baseDir, relativePath);

        if (!Directory.Exists(finalPath))
            _ = Directory.CreateDirectory(finalPath);

        return finalPath;
    }
}
