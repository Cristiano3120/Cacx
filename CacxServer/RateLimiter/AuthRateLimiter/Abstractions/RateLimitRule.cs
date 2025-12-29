namespace CacxServer.RateLimiter.AuthRateLimiter.Abstractions;

public sealed record RateLimitRule(string Key, int Limit, TimeSpan Ttl);
