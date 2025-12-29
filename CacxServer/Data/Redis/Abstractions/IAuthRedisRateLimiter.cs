using CacxServer.RateLimiter.AuthRateLimiter.Abstractions;

namespace CacxServer.Data.Redis.Abstractions;

public interface IAuthRedisRateLimiter
{
    Task<AuthRateLimitResult> CheckRulesAsync(IEnumerable<RateLimitRule> rateLimitRules);
}
