using System.Diagnostics.CodeAnalysis;

namespace CacxClient.Abstractions.Auth;

public sealed record LoginResult
{
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    public string? ErrorMessage { get; init; }
}