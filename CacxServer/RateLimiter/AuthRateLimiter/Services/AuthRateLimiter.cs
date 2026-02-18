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
        if (CheckIfSecurityDataValid(securityContext))
        {
            logger.LogWarning(LoggerParams.None, () => "IP or DeviceID hidden?? Decline request");
            return new AuthRateLimitResult(IsLimited: true, RetryAfter: TimeSpan.Zero);
        }

        (string ipHash, string deviceHash) = HashAndFormatCSC(securityContext);
        IEnumerable<RateLimitRule> rateLimitRules = RateLimitRuleBuilder.BuildRegisterRules(ipHash, deviceHash);
        
        return await authRedisRateLimiter.CheckRulesAsync(rateLimitRules);
    }

    public async Task<AuthRateLimitResult> CheckLoginAsync(ClientSecurityContext securityContext, string username)
    {
        if (CheckIfSecurityDataValid(securityContext))
        {
            logger.LogWarning(LoggerParams.None, () => "IP or DeviceID hidden?? Decline request");
            return new AuthRateLimitResult(IsLimited: true, RetryAfter: TimeSpan.Zero);
        }

        (string ipHash, string deviceHash) = HashAndFormatCSC(securityContext);
        IEnumerable<RateLimitRule> rateLimitRules = RateLimitRuleBuilder.BuildLoginRules(ipHash, deviceHash, username);
        
        return await authRedisRateLimiter.CheckRulesAsync(rateLimitRules);
    }

    public async Task<AuthRateLimitResult> CheckResendVerificationEmailAsync(ClientSecurityContext securityContext)
    { 
        if (CheckIfSecurityDataValid(securityContext))
        {
            logger.LogWarning(LoggerParams.None, () => "IP or DeviceID hidden?? Decline request");
            return new AuthRateLimitResult(IsLimited: true, RetryAfter: TimeSpan.Zero);
        }

        (_, string deviceHash) = HashAndFormatCSC(securityContext);
        IEnumerable<RateLimitRule> rateLimitRules = RateLimitRuleBuilder.BuildResendVerificationEmailRules(deviceHash);

        return await authRedisRateLimiter.CheckRulesAsync(rateLimitRules);
    }

    public async Task<AuthRateLimitResult> CheckVerifyCodeAsync(ClientSecurityContext securityContext)
    {
        if (CheckIfSecurityDataValid(securityContext))
        {
            logger.LogWarning(LoggerParams.None, () => "IP or DeviceID hidden?? Decline request");
            return new AuthRateLimitResult(IsLimited: true, RetryAfter: TimeSpan.Zero);
        }

        (string ipHash, string deviceHash) = HashAndFormatCSC(securityContext);
        IEnumerable<RateLimitRule> rateLimitRules = RateLimitRuleBuilder.BuildVerifyCodeRules(ipHash, deviceHash);

        return await authRedisRateLimiter.CheckRulesAsync(rateLimitRules);
    }

    private static bool CheckIfSecurityDataValid(ClientSecurityContext securityContext)
        => string.IsNullOrEmpty(securityContext.ClientIP?.ToString()) || string.IsNullOrEmpty(securityContext.DeviceID);

    private (string ipHash, string deviceHash) HashAndFormatCSC(ClientSecurityContext securityContext)
        =>  (
                ipHash: Convert.ToHexString(hashingService.Hash(securityContext.ClientIP?.ToString() ?? "")),
                deviceHash: Convert.ToHexString(hashingService.Hash(securityContext.DeviceID))
            );
}
