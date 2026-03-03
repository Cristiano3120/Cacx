using CacxServer.Abstractions.Auth.Verification;
using CacxServer.Data.Redis.Abstractions;
using CacxServer.Data.Redis.Entities;
using StackExchange.Redis;
using CacxShared.Abstractions;

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
            new(nameof(PendingAuthentication.DisplayName), pendingAuthentication.DisplayName),
            new(nameof(PendingAuthentication.Password), pendingAuthentication.Password),
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
            // Token expired... User needs to restart the registration process
            return VerificationResult.Fail(canRetry: false, codeExpired: true);
        }

        if (storedCode == enteredCode)
        {
            RedisValue email = await _database.HashGetAsync(formattedToken, nameof(PendingAuthentication.Email)); 
            RedisValue username = await _database.HashGetAsync(formattedToken, nameof(PendingAuthentication.Username));
            RedisValue displayName = await _database.HashGetAsync(formattedToken, nameof(PendingAuthentication.DisplayName));
            RedisValue password = await _database.HashGetAsync(formattedToken, nameof(PendingAuthentication.Password));

            // Token expired... User needs to restart the registration process
            if (email.IsNullOrEmpty || username.IsNullOrEmpty || password.IsNullOrEmpty || displayName.IsNullOrEmpty)
            {
                return VerificationResult.Fail(canRetry: false, codeExpired: true);
            }

            User user = new()
            {
                Email = email!,
                Username = username!,
                Password = password!,
                DisplayName = displayName!,
            };

            return VerificationResult.Success(user);
        }

        RedisValue attempts = await _database.HashGetAsync(formattedToken, nameof(PendingAuthentication.Attempts));
        if (!attempts.HasValue || !attempts.TryParse(out int attemptsInt))
        {
            // Token expired... User needs to restart the registration process
            return VerificationResult.Fail(canRetry: false, codeExpired: false);
        }

        if (++attemptsInt >= MaxAttempts)
        {
            //The user used up all their attempts. A new verification code has to be generated
            return VerificationResult.Fail(canRetry: true, codeExpired: true);
        }

        // Update the attempts count in Redis
        _ = await _database.HashSetAsync(formattedToken, nameof(PendingAuthentication.Attempts), attemptsInt);
        
        // Everything is valid the user just entered the wrong code
        return VerificationResult.Fail(canRetry: true, codeExpired: false);
    }
}