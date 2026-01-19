using CacxShared.Abstractions;
using System.Net;

namespace CacxClient.Services;

public sealed class ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public ApiError? ApiError { get; init; }
    public DateTimeOffset? RetryAfter { get; init; }

    public static ApiResponse<T> Ok(T data, bool isSuccess)
        => new() { IsSuccess = isSuccess, Data = data };

    public static ApiResponse<T> Error(HttpStatusCode statusCode, string message)
    {
        return new()
        {
            IsSuccess = false,
            ApiError = new ApiError
            {
                StatusCode  = statusCode,
                Message = message
            }
        };
    }

    public static ApiResponse<T> Error(HttpStatusCode statusCode, string message, DateTimeOffset? retryAfter)
    {
        return new()
        {
            IsSuccess = false,
            RetryAfter = retryAfter,
            ApiError = new ApiError
            {
                StatusCode = statusCode,
                Message = message
            }
        };
    }
}
