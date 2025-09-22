using System.Diagnostics.CodeAnalysis;

namespace CacxShared.ApiResources;

public sealed record ApiResponse<T>
{
    [MemberNotNullWhen(true, nameof(Data))]
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public ApiError Error { get; init; }

    public ApiResponse(bool isSuccess, T data)
    {
        IsSuccess = isSuccess;
        Data = data;
    }

    public ApiResponse() { }
}
