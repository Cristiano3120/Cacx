using System.Diagnostics.CodeAnalysis;

namespace CacxClient.Abstractions.Auth;

public sealed record RegisterResult
{
    [MemberNotNullWhen(true, nameof(Token))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    public string? Token { get; init; }
    public string? ErrorMessage { get; init; }
}