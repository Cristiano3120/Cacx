using System.Diagnostics.CodeAnalysis;
using CacxShared.Abstractions;

namespace CacxServer.Abstractions.Auth.Verification;

public sealed record VerificationResult
{
    [MemberNotNullWhen(true, nameof(AuthenticatedUser))]
    public bool IsSuccess => AuthenticatedUser is not null;
    public bool CanRetry { get; init; }
    public bool CodeExpired {  get; init; }
    public User? AuthenticatedUser { get; init; }

    public VerificationResult(bool canRetry, bool codeExpired, User? user)
    {
        CanRetry = canRetry;
        CodeExpired = codeExpired;
        AuthenticatedUser = user;
    }

    public static VerificationResult Success(User? pendingAuthentication) 
        => new(canRetry: false, codeExpired: false, pendingAuthentication);
    public static VerificationResult Fail(bool canRetry, bool codeExpired) 
        => new(canRetry, codeExpired, user: null);
}