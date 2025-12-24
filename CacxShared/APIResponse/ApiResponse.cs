using System.Net;

namespace CacxShared.APIResponse;

public sealed class ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public ApiError? ApiError { get; init; }

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
}
