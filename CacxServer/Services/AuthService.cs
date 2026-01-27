using CacxServer.Abstractions;
using CacxServer.Abstractions.Auth;
using CacxServer.Abstractions.Auth.Register;
using CacxServer.Data.PostgreSQL.Abstractions;
using CacxServer.Data.Redis.Abstractions;
using CacxServer.Data.Redis.Entities;
using CacxServer.Security.Hashing.Abstractions;
using Cristiano3120.Logging;
using Npgsql;
using StackExchange.Redis;
using System.Net.Mail;
using RegisterRequest = CacxShared.Abstractions.RegisterRequest;

namespace CacxServer.Services;

public class AuthService(
    [FromKeyedServices(HashingAlgorithm.Sha256)] IHashingService hashingService,
    IVerificationTokenService verificationTokenService, 
    INotificationService notificationService, 
    IAuthRedisService authRedisService,
    IAuthRepository authRepository,
    Logger logger) : IAuthService
{
    public async Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest)
    {
        CallerInfos callerInfos = CallerInfos.Create();
        try
        {
            if (await authRepository.CheckIfUniqueDataExistsAsync(registerRequest.Email, registerRequest.Username))
            {
                return RegisterResult.Fail(RegisterError.EmailOrUsernameTaken);
            }

            TimeSpan expiry = TimeSpan.FromMinutes(15);
            int verificationCode = verificationTokenService.GenerateVerificationCode();
            string token = verificationTokenService.GenerateVerificationToken();

            PendingAuthentication pendingAuthentication = new()
            {
                Email = registerRequest.Email,
                Username = registerRequest.Username,
                VerificationCode = hashingService.Hash(verificationCode.ToString())
            };

            string hashedToken = FormatToken(token);
            bool redisEntrySuccessful = await authRedisService.TryAddPendingVerificationAsync(hashedToken,
                pendingAuthentication, expiry);

            if (!redisEntrySuccessful)
            {
                return RegisterResult.Fail(RegisterError.PendingReservationExists);
            }

            await SendVerificationEmailAsync(registerRequest.Email, verificationCode);

            return RegisterResult.Success(token);
        }
        catch (RedisException)
        {
            logger.LogError(LoggerParams.None, () => "Redis not available", callerInfos);
            return RegisterResult.Fail(RegisterError.ServiceUnavailable);
        }
        catch (NpgsqlException)
        {
            logger.LogError(LoggerParams.None, () => "PostgreSQL not available", callerInfos);
            return RegisterResult.Fail(RegisterError.ServiceUnavailable);
        }
        catch (SmtpException)
        {
            logger.LogError(LoggerParams.None, () => "Notification not available", callerInfos);
            return RegisterResult.Fail(RegisterError.NotificationFailed);
        }
        catch (Exception ex)
        {
            logger.LogError(LoggerParams.None, ex, callerInfos);
            return RegisterResult.Fail(RegisterError.Unknown);
        }
    }

    public async Task<bool> ResendVerificationEmailAsync(string authToken)
    {
        int verificationCode = verificationTokenService.GenerateVerificationCode();
        string? email = await authRedisService.ReplaceVerificationCodeAndGetEmailAsync(FormatToken(authToken), verificationCode);

        if (email is null)
            return false;

        await SendVerificationEmailAsync(email, verificationCode);
        return true;
    }

    private async Task SendVerificationEmailAsync(string email, int verificationCode)
    {
        string subject = "[CACX]: Verification";
        string body = $"Hello {email} 👋 \nHere is your verification code: {verificationCode}. Make sure to be quick it will expire soon!";
        await notificationService.SendEmailAsync(targetEmails: [email], subject, body);
    }

    private string FormatToken(string token)
        => Convert.ToHexStringLower(hashingService.Hash(token));
}
