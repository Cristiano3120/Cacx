using CacxServer.Data.Redis.Abstractions;
using CacxServer.RateLimiter.AuthRateLimiter.Abstractions;
using Cristiano3120.Logging;
using StackExchange.Redis;

namespace CacxServer.Data.Redis.Services;

public sealed class AuthRedisRateLimiter(IConnectionMultiplexer connectionMultiplexer, Logger logger) : IAuthRedisRateLimiter
{
    private readonly IDatabase _db = connectionMultiplexer.GetDatabase();

    public async Task<AuthRateLimitResult> CheckRulesAsync(IEnumerable<RateLimitRule> rateLimitRules)
    {
        try
        {
            foreach (RateLimitRule rule in rateLimitRules)
            {
                // Increment counter (Atomic)
                long count = await _db.StringIncrementAsync(rule.Key);

                // If the key didn´t exist before -> set a ttl (expiry)
                if (count == 1)
                {
                    _ = await _db.KeyExpireAsync(rule.Key, rule.Ttl);
                }

                // If count exceeds limit -> block 
                if (count > rule.Limit)
                {
                    DateTime? dateTime = await _db.KeyExpireTimeAsync(rule.Key);
                    if (dateTime is not null)
                    {
                        TimeSpan waitTime = dateTime.Value - DateTime.UtcNow;
                        return waitTime > TimeSpan.Zero 
                            ? new AuthRateLimitResult(IsLimited: true, RetryAfter: waitTime)
                            : new AuthRateLimitResult(IsLimited: false, RetryAfter: TimeSpan.Zero); // Time already expired
                    }

                    return new AuthRateLimitResult(IsLimited: true, RetryAfter: rule.Ttl);
                }
            }

            return new AuthRateLimitResult(IsLimited: false, RetryAfter: TimeSpan.Zero);
        }
        catch (RedisException)
        {
            logger.LogWarning(LoggerParams.None, () => "Redis down :( Not checking auth requests!");
            return new AuthRateLimitResult(IsLimited: false, RetryAfter: TimeSpan.Zero);
        }
    }
}
