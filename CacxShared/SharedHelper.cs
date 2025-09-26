using System.Runtime.CompilerServices;

namespace CacxShared;

public static class SharedHelper
{
    /// <summary>
    /// Resolves an absolute file system path based on a relative path from the project root directory at runtime.
    /// </summary>
    /// <remarks>This method is useful for locating files or directories relative to the project root,
    /// especially in scenarios where the application is running from a build output directory such as 'bin'. The method
    /// assumes the presence of a 'bin' directory in the application's base path to identify the project root.</remarks>
    /// <param name="relativePath">The relative path from the project root directory to combine with the base path. Cannot be null or empty.</param>
    /// <returns>A string containing the absolute path corresponding to the specified relative path from the project root.</returns>
    /// <exception cref="Exception">Thrown if the project base path cannot be determined from the current application context.</exception>
    public static string GetDynamicPath(string relativePath)
    {
        string projectBasePath = AppContext.BaseDirectory;
        int binIndex = projectBasePath.IndexOf($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal);

        if (binIndex == -1)
        {
            throw new InvalidOperationException("Could not determine project base path!");
        }

        projectBasePath = projectBasePath[..binIndex];
        return Path.Combine(projectBasePath, relativePath);
    }

    /// <summary>
    /// Takes an all caps string and turns it into a normal string.
    /// <para>
    /// <c>Turns "GET" int "Get"</c>
    /// </para>
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string FromUpperToNormal(this string str)
    {
        str = str.ToLower();
        return str[0].ToString().ToUpper() + str[1..];
    }
}
