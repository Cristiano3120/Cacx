using CacxServer.Data.Redis.Entities;

namespace CacxServer.Data.Redis.Abstractions;

public interface IAuthRedisService
{
    Task<bool> TryAddPendingVerificationAsync(string tokenHash, PendingAuthentication pendingAuthentication, TimeSpan expiry);
    Task<string?> ReplaceVerificationCodeAndGetEmailAsync(string tokenHash, int newVerificationCode);
    Task CheckVerificationCodeAsync(int code);
}
