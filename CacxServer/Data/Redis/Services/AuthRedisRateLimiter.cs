using CacxServer.Data.Redis.Abstractions;
using CacxServer.RateLimiter.AuthRateLimiter.Abstractions;
using CacxServer.Security.Hashing;
using StackExchange.Redis;

namespace CacxServer.Data.Redis.Services;

public sealed class AuthRedisRateLimiter(
    [FromKeyedServices(HashingAlgorithm.Sha256)] IHashingService hashingService,
    IConnectionMultiplexer connectionMultiplexer) : IAuthRedisRateLimiter
{
    private readonly IDatabase _db = connectionMultiplexer.GetDatabase();

    public async Task<bool> CheckRulesAsync(IEnumerable<RateLimitRule> rateLimitRules)
    {
        //TODO: Implement
        throw new NotImplementedException(nameof(CheckRulesAsync));
    }
}
