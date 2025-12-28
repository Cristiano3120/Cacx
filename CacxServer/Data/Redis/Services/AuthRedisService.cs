using CacxServer.Data.Redis.Abstractions;
using CacxServer.Data.Redis.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace CacxServer.Data.Redis.Services;

public class AuthRedisService(IConnectionMultiplexer connectionMultiplexer) : IAuthRedisService
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    public async Task<bool> TryAddPendingVerificationAsync(string token, 
        PendingAuthentication pendingAuthentication, TimeSpan expiry)
    {
        string json = JsonSerializer.Serialize(pendingAuthentication);

        return await _database.StringSetAsync(
            key: new RedisKey(token), 
            value: new RedisValue(json),
            expiry,
            when: When.NotExists
        );
    }

    public async Task CheckVerificationCodeAsync(int code)
    {
        //TODO: erhöhe attempts
        //TODO: Verify hash
    }
}
