using CacxClient.Interfaces;

namespace CacxClient.Services.RateLimiter;

public class RateLimiters
{
    public IRateLimiter Login { get; } = new TokenBucket(regenerationRate: TimeSpan.FromSeconds(3), maxTokens: 2);
}
