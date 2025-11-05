using CacxShared.SharedMinioResources;

namespace CacxShared.Endpoints;

public static class Endpoints
{
    public const string BaseEndpoint = "api";
    public const string BaseAuthEndpoint = "auth";
    public const string BaseCdnEndpoint = "cdn";

    /// <summary>
    /// Use the <see cref="AuthEndpoint"/> class for the param
    /// </summary>
    /// <param name="authEndpoint">One of the constants implemented in the <see cref="AuthEndpoint"/> class</param>
    /// <returns></returns>
    public static string GetAuthEndpoint(string authEndpoint)
    {
        return $"{BaseEndpoint}/{BaseAuthEndpoint}/{authEndpoint}";
    }

    /// <summary>
    /// Use the <see cref="AuthEndpoint"/> class for the param
    /// </summary>
    /// <param name="resourcePath"> the path to the resource you wanna get. Example: 148735781742/profilePicture.png</param>
    /// <param name="bucketName">One of the constants implemented in the <see cref="Bucket"/> class</param>

    /// <returns></returns>
    public static string GetCdnEndpoint(Bucket bucketName, string resourcePath)
    { 
        return $"{BaseCdnEndpoint}/{bucketName.ToBucketName()}/{resourcePath}";
    }
}
