namespace CacxServer.Abstractions.Auth.Verification;

public sealed record VerificationResult(bool IsSuccess, bool CanRetry, VerificationError VerificationError = VerificationError.None)
{
    internal static VerificationResult Fail(VerificationError verificationError) 
        => new(IsSuccess: false, CanRetry: true, verificationError);
}