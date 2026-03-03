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
    //MUSST WAS ANDERES RETURNEN
    public async Task<(bool isSuccess, bool canRetry, bool codeExpired)> VerifyAsync(string authToken, string deviceID, int code)
    {
        CallerInfos callerInfos = CallerInfos.Create();
        try
        {
            VerificationResult verificationResult = await authRedisService.CheckVerificationCodeAsync(formattedToken: authToken, code);
            if (!verificationResult.IsSuccess)
            {
                //TODO: EfCore benchmarken maybe warmups oder dapper
                //TODO: Delete n paar unnötige Interfaces
                //TODO: Update HashingService mach mehr methods z.B eine die instant str returnt und maybe Argon statt Bcrpyt
                //maybe user factory die nen Encrypted user erstellen kann und nen normalen und nen decrypteten usw
                //TODO: Issues in GitHub bundlen
                //TODO: Code Cleanup. Fix AuthService methods + mehr logging und comments
                return (isSuccess: false, canRetry: verificationResult.CanRetry, codeExpired: verificationResult.CodeExpired);
            }

            long id = snowflakeGenerator.NextId();
            JwtTokens jwtTokens = JwtTokenGenerator.GenerateJwtTokens(id, deviceID);
            bool userAdded = await authRepository.AddUserAsync(verificationResult.AuthenticatedUser, jwtTokens.RefreshToken, id);

            if (userAdded)
            {
                return (isSuccess: true, canRetry: false, codeExpired: false);
            }
        }
        catch (RedisException) //Musst die fehler einzelnt handlen oder maybe null returnen
        {
            logger.LogError(LoggerParams.None, () => "Redis not available", callerInfos);
        }
        catch (NpgsqlException)
        {
            logger.LogError(LoggerParams.None, () => "PostgreSQL not available", callerInfos);
        }
        catch (Exception ex)
        {
            logger.LogError(LoggerParams.None, ex, callerInfos);
        }
    }

    /// <param name="authToken"></param>
    /// <returns>
    /// <see cref="TimeSpan.Zero"/> <see langword="if"/> something went wrong
    /// <see langword="else"/> > <see cref="TimeSpan.Zero"/>
    /// </returns>
    public async Task<ResendVerificationResult> ResendVerificationEmailAsync(string authToken)
    {
        int verificationCode = verificationTokenService.GenerateVerificationCode();
        string? email = await authRedisService.ReplaceVerificationCodeAndGetEmailAsync(FormatToken(authToken), verificationCode);

        if (email is null)
            return ResendVerificationResult.SessionInvalid;

        if (!await SendVerificationEmailAsync(email, verificationCode))
        {
            return ResendVerificationResult.EmailSendFailed;
        }

        return ResendVerificationResult.Success;
    }

    private async Task<bool> SendVerificationEmailAsync(string email, int verificationCode)
    {
        string subject = "[CACX]: Verification";
        string body = $"Hello {email} 👋 \nHere is your verification code: {verificationCode}. Make sure to be quick it will expire soon!";
        return await notificationService.SendEmailAsync(targetEmails: [email], subject, body);
    }

    private string FormatToken(string token)
        => Convert.ToHexStringLower(shaHashingService.Hash(token));
}