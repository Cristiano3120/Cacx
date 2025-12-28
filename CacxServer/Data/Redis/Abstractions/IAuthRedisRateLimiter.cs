using CacxServer.RateLimiter.AuthRateLimiter.Abstractions;

namespace CacxServer.Data.Redis.Abstractions;

public interface IAuthRedisRateLimiter
{
    Task<bool> CheckRulesAsync(IEnumerable<RateLimitRule> rateLimitRules);
}
