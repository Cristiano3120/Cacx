using System.Net;

namespace CacxShared.APIResponse;

public sealed record ApiError
{
    public HttpStatusCode StatusCode { get; init; }
    public string? Message { get; init; }
}