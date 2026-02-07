using CacxServer.Data.Redis.Abstractions;
using CacxServer.Data.Redis.Entities;
using StackExchange.Redis;

namespace CacxServer.Data.Redis.Services;

public sealed class AuthRedisService(IConnectionMultiplexer connectionMultiplexer) : IAuthRedisService
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    public async Task<bool> TryAddPendingVerificationAsync(
        string tokenHash, 
        PendingAuthentication pendingAuthentication, 
        TimeSpan expiry)
    {
        HashEntry[] hashEntries =
        [
            new(nameof(PendingAuthentication.Email), pendingAuthentication.Email),
            new(nameof(PendingAuthentication.Username), pendingAuthentication.Username),
            new(nameof(PendingAuthentication.VerificationCode), pendingAuthentication.VerificationCode),
            new(nameof(PendingAuthentication.Attempts), pendingAuthentication.Attempts)
        ];

        ITransaction tran = _database.CreateTransaction();

        _ = tran.AddCondition(Condition.KeyNotExists($"pending:email:{pendingAuthentication.Email}"));
        _ = tran.AddCondition(Condition.KeyNotExists($"pending:username:{pendingAuthentication.Username}"));
        
        _ = tran.HashSetAsync(tokenHash, hashEntries);
        _ = tran.KeyExpireAsync(tokenHash, expiry);

        _ = tran.StringSetAsync($"pending:email:{pendingAuthentication.Email}", tokenHash, expiry);
        _ = tran.StringSetAsync($"pending:username:{pendingAuthentication.Username}", tokenHash, expiry);

        //False if any of the conditions fail, otherwise true
        return await tran.ExecuteAsync();
    }

    public async Task<string?> ReplaceVerificationCodeAndGetEmailAsync(string tokenHash, int newVerificationCode)
    {
        RedisValue emailValue = await _database.HashGetAsync(tokenHash, nameof(PendingAuthentication.Email));

        if (!emailValue.HasValue)
            return null; // Token expired...

        _ = await _database.HashSetAsync(tokenHash, nameof(PendingAuthentication.VerificationCode), newVerificationCode);

        return emailValue;
    }

    public async Task CheckVerificationCodeAsync(int code)
    {
        //TODO: erhöhe attempts
        //TODO: Verify hash
    }
}
