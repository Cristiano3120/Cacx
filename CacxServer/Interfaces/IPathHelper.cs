using CacxServer.Helper;

namespace CacxServer.Interfaces;

public interface IPathHelper
{
    string GetPath(PathType type);
}