using CacxServer.Abstractions;
using CacxServer.Abstractions.Auth;
using CacxServer.Abstractions.Auth.Register;
using CacxServer.Abstractions.Auth.Verification;
using CacxServer.Data.PostgreSQL.Abstractions;
using CacxServer.Data.Redis.Abstractions;
using CacxServer.Data.Redis.Entities;
using CacxServer.Security.Hashing.Abstractions;
using CacxShared.Abstractions;
using Cristiano3120.Logging;
using Npgsql;
using StackExchange.Redis;
using System.Net.Mail;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RegisterRequest = CacxShared.Abstractions.RegisterRequest;

namespace CacxServer.Services;

public class AuthService(
    [FromKeyedServices(HashingAlgorithm.BCrypt)] IHashingService bcryptHashingService,
    [FromKeyedServices(HashingAlgorithm.Sha256)] IHashingService shaHashingService,
    IVerificationTokenService verificationTokenService, 
    INotificationService notificationService, 
    ISnowflakeGenerator snowflakeGenerator,
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
                Password = registerRequest.Password,
                DisplayName = Convert.ToHexString(bcryptHashingService.Hash(registerRequest.Username)),
                VerificationCode = shaHashingService.Hash(verificationCode.ToString())
            };

            string hashedToken = FormatToken(token);
            bool redisEntrySuccessful = await authRedisService.TryAddPendingVerificationAsync(
                hashedToken,
                pendingAuthentication, 
                expiry);

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
            logger.LogError(LoggerParams.None, () => "Notification service not available", callerInfos);
            return RegisterResult.Fail(RegisterError.NotificationFailed);
        }
        catch (Exception ex)
        {
            logger.LogError(LoggerParams.None, ex, callerInfos);
            Environment.Exit(1);
            return RegisterResult.Fail(RegisterError.Unknown);
        }
    }

    public async Task<VerificationResult> VerifyAsync(string authToken, string deviceID, int code)
    {
        CallerInfos callerInfos = CallerInfos.Create();
        try
        {
            VerificationResult verificationResult = await authRedisService.CheckVerificationCodeAsync(formattedToken: authToken, code);
            if (verificationResult.IsSuccess)
                //TODO: Delete n paar unnötige Interfaces
                //TODO: Update HashingService mach mehr methods z.B eine die instant str returnt und maybe Argon statt Bcrpyt
                //TODO: User soll displayName und password mitschicken vorher schon bei acc ersdtellung
                //TODO: Überlege was besseres als Enviroment.Exit. Server soll was returnen was der Client versteht
            {   //maybe user factory die nen Encrypted user erstellen kann und nen normalen und nen decrypteten usw
                // TODO: GENERATE ID AND CREATE USER IN DB DO THAT ON ANOTHER THREAD. Maybe add JwtToken zur User class
                //TODO: GUcken was du responden musst
                long userID = snowflakeGenerator.GenerateId();
                JwtTokens jwtTokens = JwtTokenGenerator.GenerateJwtTokens(userID, deviceID); 

                authRepository.AddUser(); //TODO: Implement method
                return default!;
            }
        }
        catch (RedisException)
        {
            logger.LogError(LoggerParams.None, () => "Redis not available", callerInfos);
            return VerificationResult.Fail(VerificationError.RedisUnavailable);
        }
        catch (Exception ex)
        {
            logger.LogError(LoggerParams.None, ex, callerInfos);
            Environment.Exit(1);
            return VerificationResult.Fail(VerificationError.UnknownError);
        }

        return default!;
    }

    /// <param name="authToken"></param>
    /// <returns>
    /// <see cref="TimeSpan.Zero"/> <see langword="if"/> something went wrong
    /// <see langword="else"/> > <see cref="TimeSpan.Zero"/>
    /// </returns>
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
        => Convert.ToHexStringLower(shaHashingService.Hash(token));
}
