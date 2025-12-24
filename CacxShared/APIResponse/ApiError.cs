using System.Net;

namespace CacxShared.APIResponse;

public record ApiError
{
    public HttpStatusCode StatusCode { get; init; }
    public string Message { get; init; }

    public ApiError(HttpStatusCode statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }
}