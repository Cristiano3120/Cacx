using CacxServer.Abstractions.Auth.Verification;
using CacxServer.Data.Redis.Abstractions;
using CacxServer.Data.Redis.Entities;
using StackExchange.Redis;

namespace CacxServer.Data.Redis.Services;

public sealed class AuthRedisService(IConnectionMultiplexer connectionMultiplexer) : IAuthRedisService
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    public async Task<bool> TryAddPendingVerificationAsync(
        string formattedToken, 
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
        
        _ = tran.HashSetAsync(formattedToken, hashEntries);
        _ = tran.KeyExpireAsync(formattedToken, expiry);

        _ = tran.StringSetAsync($"pending:email:{pendingAuthentication.Email}", formattedToken, expiry);
        _ = tran.StringSetAsync($"pending:username:{pendingAuthentication.Username}", formattedToken, expiry);

        //False if any of the conditions fail, otherwise true
        return await tran.ExecuteAsync();
    }

    public async Task<string?> ReplaceVerificationCodeAndGetEmailAsync(string formattedToken, int newVerificationCode)
    {
        RedisValue emailValue = await _database.HashGetAsync(formattedToken, nameof(PendingAuthentication.Email));

        if (!emailValue.HasValue)
            return null; // Token expired...

        _ = await _database.HashSetAsync(formattedToken, nameof(PendingAuthentication.VerificationCode), newVerificationCode);

        return emailValue;
    }

    public async Task<VerificationResult> CheckVerificationCodeAsync(string formattedToken, int enteredCode)
    {
        RedisValue verificationCodeField = await _database.HashGetAsync(formattedToken, nameof(PendingAuthentication.VerificationCode));
        const int MaxAttempts = 5;

        if (!verificationCodeField.HasValue || !verificationCodeField.TryParse(out int storedCode))
        {
            return new VerificationResult(IsSuccess: false, CanRetry: false); // Token expired...
        }

        if (storedCode == enteredCode)
        {
            return new VerificationResult(IsSuccess: true, CanRetry: false);
        }

        RedisValue attempts = await _database.HashGetAsync(formattedToken, nameof(PendingAuthentication.Attempts));
        if (!attempts.HasValue || !attempts.TryParse(out int attemptsInt))
        {
            return new VerificationResult(IsSuccess: false, CanRetry: false); // Token expired...
        }

        if (++attemptsInt <= MaxAttempts)
        {
            return new VerificationResult(IsSuccess: false, CanRetry: true, VerificationError.TooManyAttempts);
        }

        _ = await _database.HashSetAsync(formattedToken, nameof(PendingAuthentication.Attempts), attemptsInt);
        return new VerificationResult(IsSuccess: false, CanRetry: true);
    }
}