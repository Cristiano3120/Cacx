using System.Net;

namespace CacxShared.ApiResources;

public readonly record struct ApiError
{
    public HttpStatusCode StatusCode { get; init; }
    public string Message { get; init; }
}
