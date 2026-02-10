using CacxServer.Abstractions.Auth.Verification;
using CacxServer.Data.Redis.Entities;

namespace CacxServer.Data.Redis.Abstractions;

public interface IAuthRedisService
{
    Task<bool> TryAddPendingVerificationAsync(string formattedToken, PendingAuthentication pendingAuthentication, TimeSpan expiry);
    Task<string?> ReplaceVerificationCodeAndGetEmailAsync(string formattedToken, int newVerificationCode);
    Task<VerificationResult> CheckVerificationCodeAsync(string formattedToken, int enteredCode);
}