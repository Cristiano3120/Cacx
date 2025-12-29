namespace CacxServer.RateLimiter.AuthRateLimiter.Abstractions;

public record struct AuthRateLimitResult(bool IsLimited, TimeSpan RetryAfter);