using System.Net;

namespace CacxShared.APIResponse;

public sealed class ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public ApiError? ApiError { get; init; }

    public static ApiResponse<T> Ok(T data, bool isSucces)
        => new() { IsSuccess = isSucces, Data = data };

    public static ApiResponse<T> Error(HttpStatusCode statusCode, string message)
        => new() { IsSuccess = false, ApiError = new ApiError(statusCode, message) };
}
