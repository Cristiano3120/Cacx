using CacxShared.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;

namespace CacxClient.Abstractions;

public sealed record ApiResponse<T>
{
    /// <summary>
    /// Gets a value indicating whether the request completed successfully.
    /// The value can be <see langword="true"/> even if the operation itself did not succeed,
    /// </summary>
    /// <remarks>If this property is <see langword="false"/>, the <c>ApiError</c> property will contain
    /// details about the error that occurred during the operation.</remarks>
    [MemberNotNullWhen(false, nameof(ApiError))]
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
