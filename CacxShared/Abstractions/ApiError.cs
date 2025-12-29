using System.Net;

namespace CacxShared.Abstractions;

public sealed record ApiError
{
    public required HttpStatusCode StatusCode { get; init; }
    public required string Message { get; init; }
}