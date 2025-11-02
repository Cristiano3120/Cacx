using DotNetEnv;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using CacxShared.SharedDTOs;
using Cristiano3120.Logging;
using CacxServer.UserDataDatabaseResources;
using CacxShared.ApiResources;

namespace CacxServer.Services;

public sealed class AuthService(UserDataDatabase userDataDatabase, Logger logger)
{
    private readonly Dictionary<string, (int verificationCode, Timer timer)> _verificationData = [];

    public async Task<ApiResponse<(bool emailFound, bool usernameFound)>> CheckEmailAndUsernameAsync(UniqueUserData uniqueUserData)
    {
        //Check for uniqueness in the db
        DatabaseResult<(bool emailFound, bool usernameFound)> databaseResult
            = await userDataDatabase.CheckEmailAndUsernameAsync(uniqueUserData);

        if (!databaseResult.RequestSuccessful)
        {
            return new ApiResponse<(bool emailFound, bool usernameFound)>
            {
                IsSuccess = false,
                Error = new ApiError()
                {
                    Message = "Some database problem!",
                    StatusCode = HttpStatusCode.InternalServerError
                }
            };
        }

        (bool emailFound, bool usernameFound) = databaseResult.ReturnedValue;
        return new ApiResponse<(bool emailFound, bool usernameFound)>()
        {
            IsSuccess = true,
            Data = (emailFound, usernameFound)
        };
    }

    public int CreateAndSaveCode(string username)
    {
        int code = RandomNumberGenerator.GetInt32(int.MaxValue);
        Timer timer = new(DeleteVerificationCode, username, TimeSpan.FromMinutes(5), Timeout.InfiniteTimeSpan);
        
        _verificationData[username] = (code, timer);

        return code;
    }

    public bool CheckVerificationCode(string username, int verificationCode)
    {
        if (!_verificationData.TryGetValue(username, out (int, Timer) value))
        {
            return false;
        }

        if (verificationCode != value.Item1)
        {
            return false;
        }

        _ = _verificationData.Remove(username);
        value.Item2.Dispose();

        return true;
    }

    private void DeleteVerificationCode(object? state)
    {
        string username = state as string ?? "";

        (_, Timer timer) = _verificationData[username];
        _ = _verificationData.Remove(username);

        timer.Dispose();
    }

    public async Task<User?> SaveUserToDbAsync(User user)
    {
        User idUser = user with
        {
            Id = SnowflakeGenerator.Generate()
        };

        DatabaseResult<object> databaseResult 
            = await userDataDatabase.AddUserToDbAsync(idUser);

        if (databaseResult.RequestSuccessful)
        {
            return idUser with
            {
                Password = ""
            };
        }

        return null;
    }

    public async Task<bool> SendVerificationEmailAsync(UniqueUserData uniqueUserData, int verificationCode)
    {
#if !DEBUG
        try
        {
            // Use SMTP to send an email
            const string EmailAttribute = "EMAIL";
            const string AppPasswordAttribute = "EMAIL_PASSWORD";

            MailMessage mail = new()
            {
                From = new MailAddress(Env.GetString(EmailAttribute)),
                To = { uniqueUserData.Email! },
                Subject = "[CACX]: Verification verificationCode",
                Body = $"Hello {uniqueUserData.Username}! Your verification code is: {verificationCode}",
            };

            using SmtpClient smtp = new("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(Env.GetString(EmailAttribute), Env.GetString(AppPasswordAttribute)),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(LoggerParams.None, ex, CallerInfos.Create());
            return false;
        } 
#else 
        logger.LogInformation(LoggerParams.None, $"code: {verificationCode}");
        return true;
#endif
    }
}
