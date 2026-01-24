using CacxShared.Abstractions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CacxClient.Services;

public sealed record ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public ApiError? ApiError { get; init; }
    public TimeSpan? RetryAfter { get; init; }

    public static ApiResponse<T> Ok(T data, bool isSuccess)
        => new() { IsSuccess = isSuccess, Data = data };

    public static ApiResponse<T> Error(HttpStatusCode statusCode, string message)
    {
        return new()
        {
            IsSuccess = false,
            ApiError = new ApiError
            {
                StatusCode = statusCode,
                Message = message
            }
        };
    }

    public static ApiResponse<T> FromHttp(ApiResponse<T> body, HttpResponseHeaders headers)
    {
        return new ApiResponse<T>
        {
            IsSuccess = body.IsSuccess,
            Data = body.Data,
            ApiError = body.ApiError,
            RetryAfter = headers.RetryAfter?.Delta
        };
    }
}
