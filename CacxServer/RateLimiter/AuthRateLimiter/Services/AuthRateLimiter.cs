using CacxServer.Abstractions.Auth;
using CacxServer.Data.Redis.Abstractions;
using CacxServer.RateLimiter.AuthRateLimiter.Abstractions;
using CacxServer.Security.Hashing;

namespace CacxServer.RateLimiter.AuthRateLimiter.Services;

public sealed class AuthRateLimiter(
    [FromKeyedServices(HashingAlgorithm.Sha256)] IHashingService hashingService,
    IAuthRedisRateLimiter authRedisRateLimiter) : IAuthRateLimiter
{
    public async Task<bool> CheckRegisterAsync(ClientSecurityContext securityContext)
    {
        string ipHash = Convert.ToBase64String(hashingService.Hash(securityContext.ClientIP?.ToString() ?? ""));
        string deviceHash = Convert.ToBase64String(hashingService.Hash(securityContext.DeviceID));

        if (ipHash.Length == 0)
        {
            Console.WriteLine("IP hidden?? Declined request");
            return false;
        }

        IEnumerable<RateLimitRule> rateLimitRules = RateLimitRuleBuilder.BuildRegisterRules(ipHash, deviceHash);
        return await authRedisRateLimiter.CheckRulesAsync(rateLimitRules);
    }

    public async Task<bool> CheckLoginAsync(ClientSecurityContext securityContext)
    {
        //TODO: Implement
        throw new NotImplementedException(nameof(CheckLoginAsync));
    }
}
