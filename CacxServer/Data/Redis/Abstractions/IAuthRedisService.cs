using CacxServer.Data.Redis.Entities;

namespace CacxServer.Data.Redis.Abstractions;

public interface IAuthRedisService
{
    Task<bool> TryAddPendingVerificationAsync(string token, PendingAuthentication pendingAuthentication, TimeSpan expiry);
    Task CheckVerificationCodeAsync(int code);
}
