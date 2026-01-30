using System.Diagnostics.CodeAnalysis;

namespace CacxClient.Abstractions.Auth;

public sealed record RequestVerificationEmailResult
{
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    public bool SessionExpired {  get; init; }
    public string? ErrorMessage { get; init; } 
}
