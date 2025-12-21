using CacxServer.Data.Redis.Abstractions;
using StackExchange.Redis;

namespace CacxServer.Data.Redis;

public class RedisService : IRedisService
{
    private readonly IDatabase _database;

    public RedisService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }
}
