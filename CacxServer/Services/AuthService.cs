using CacxServer.Abstractions;
using CacxServer.Abstractions.Auth;
using CacxServer.Data.PostgreSQL.Abstractions;
using CacxServer.Data.Redis.Abstractions;
using CacxServer.Data.Redis.Entities;
using CacxShared.Abstractions;

namespace CacxServer.Services;

public class AuthService(
    IVerificationTokenService verificationTokenService, 
    INotificationService notificationService, 
    IAuthRedisService authRedisService,
    IAuthRepository authRepository) : IAuthService
{
    public async Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest)
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
            VerificationCode = Hash(verificationCode), //Encryption service
        };
        bool redisEntrySuccessful = await authRedisService.TryAddPendingVerificationAsync(token,
            pendingAuthentication, expiry);

        if (!redisEntrySuccessful)
        {
            return RegisterResult.Fail(RegisterError.PendingReservationExists);
        }

        string subject = "[CACX]: Verification";
        string body = $"Hello {registerRequest.Username} 👋 \n Here is your verification code: {verificationCode}. Make sure to be quick it will expire soon!";
        await notificationService.SendEmailAsync(targetEmails: [registerRequest.Email], subject, body);

        return RegisterResult.Success(token);
    }
}
