using CacxServer.Abstractions.Auth;
using CacxServer.Data.Redis.Abstractions;
using CacxServer.RateLimiter.AuthRateLimiter.Abstractions;
using CacxServer.Security.Hashing.Abstractions;
using Cristiano3120.Logging;

namespace CacxServer.RateLimiter.AuthRateLimiter.Services;

public sealed class AuthRateLimiter(
    [FromKeyedServices(HashingAlgorithm.Sha256)] IHashingService hashingService,
    IAuthRedisRateLimiter authRedisRateLimiter,
    Logger logger) : IAuthRateLimiter
{

    public async Task<AuthRateLimitResult> CheckRegisterAsync(ClientSecurityContext securityContext)
    {
        string ipHash = Convert.ToHexString(hashingService.Hash(securityContext.ClientIP?.ToString() ?? ""));
        string deviceHash = Convert.ToHexString(hashingService.Hash(securityContext.DeviceID));

        if (ipHash.Length == 0)
        {
            logger.LogWarning(LoggerParams.None, () => "IP hidden?? Decline request");
            return new AuthRateLimitResult(IsLimited: true, RetryAfter: TimeSpan.Zero);
        }

        IEnumerable<RateLimitRule> rateLimitRules = RateLimitRuleBuilder.BuildRegisterRules(ipHash, deviceHash);
        return await authRedisRateLimiter.CheckRulesAsync(rateLimitRules);
    }

    public async Task<AuthRateLimitResult> CheckLoginAsync(ClientSecurityContext securityContext, string username)
    {
        string ipHash = Convert.ToHexString(hashingService.Hash(securityContext.ClientIP?.ToString() ?? ""));
        string deviceHash = Convert.ToHexString(hashingService.Hash(securityContext.DeviceID));

        if (ipHash.Length == 0)
        {
            Console.WriteLine("IP hidden?? Decline request");
            return false;
        }
       
        IEnumerable<RateLimitRule> rateLimitRules = RateLimitRuleBuilder.BuildLoginRules(ipHash, deviceHash, username);
        return await authRedisRateLimiter.CheckRulesAsync(rateLimitRules);
    }
}
