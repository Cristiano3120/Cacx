namespace CacxServer.RateLimiter.AuthRateLimiter.Abstractions;

public struct AuthRateLimitResult(bool IsLimited, TimeSpan RetryAfter);