﻿using DotNetEnv;
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

    public async Task<bool> SendVerificationEmailAsync(UniqueUserData uniqueUserData, int code)
    {
#if !DEBUG
        try
        {
            // Use SMTP to send an email
            const string EmailAttribute = "EMAIL";
            const string AppPasswordAttribute = "EMAIL_PASSWORD";
            int code = RandomNumberGenerator.GetInt32(int.MaxValue);

            MailMessage mail = new()
            {
                From = new MailAddress(Env.GetString(EmailAttribute)),
                To = { uniqueUserData.Email! },
                Subject = "[CACX]: Verification code",
                Body = $"Hello {uniqueUserData.Username}! Your verification code is: {code}",
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
        logger.LogInformation(LoggerParams.None, $"code: {RandomNumberGenerator.GetInt32(int.MaxValue)}");
        return true;
#endif
    }
}
