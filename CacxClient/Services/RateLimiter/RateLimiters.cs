using CacxClient.Abstractions;

namespace CacxClient.Services.RateLimiter;

public class RateLimiters
{
    public IRateLimiter Login { get; } = new TokenBucket(regenerationRate: TimeSpan.FromSeconds(3), maxTokens: 2);
    public IRateLimiter CreateAcc { get; } = new TokenBucket(regenerationRate: TimeSpan.FromSeconds(2), maxTokens: 3);
}
