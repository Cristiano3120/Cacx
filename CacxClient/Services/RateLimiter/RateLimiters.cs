using CacxClient.Abstractions;

namespace CacxClient.Services.RateLimiter;

public static class RateLimiters
{
    public static IRateLimiter Login => new TokenBucket(regenerationRate: TimeSpan.FromSeconds(3), maxTokens: 2);
    public static IRateLimiter Register => new TokenBucket(regenerationRate: TimeSpan.FromSeconds(5), maxTokens: 3);
    public static IRateLimiter Verification => new TokenBucket(regenerationRate: TimeSpan.FromSeconds(15), maxTokens: 3);
    public static IRateLimiter ResendVerificationEmail => new TokenBucket(regenerationRate: TimeSpan.FromSeconds(60), maxTokens: 1);
}
