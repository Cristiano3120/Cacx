namespace CacxShared.Endpoints;

public static class Endpoints
{
    public const string BaseEndpoint = "api";
    public const string BaseAuthEndpoint = "auth";

    /// <summary>
    /// Use the <see cref="AuthEndpoint"/> class for the param
    /// </summary>
    /// <param name="authEndpoint">One of the constants implemented in the <see cref="AuthEndpoint"/> class</param>
    /// <returns></returns>
    public static string GetAuthEndpoint(string authEndpoint)
    {
        return $"{BaseEndpoint}/{BaseAuthEndpoint}/{authEndpoint}";
    }
}
