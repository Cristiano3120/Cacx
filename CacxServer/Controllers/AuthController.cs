using CacxServer.Helper;
using CacxServer.Services;
using CacxShared.ApiResources;
using CacxShared.Endpoints;
using CacxShared.SharedDTOs;
using Cristiano3120.Logging;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace CacxServer.Controllers;

[ApiController]
[Route($"{Endpoints.BaseEndpoint}/{Endpoints.BaseAuthEndpoint}")]
public sealed class AuthController(AuthService authService, LoggingHelper loggingHelper, Logger logger) : ControllerBase
{
    private readonly LoggingHelper _loggingHelper = loggingHelper;
    private readonly Logger _logger = logger;

    [HttpGet($"{AuthEndpoint.Ping}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public ActionResult Ping()
    {
        ApiResponse<object> response = new() { IsSuccess = true };

        _logger.LogDebug(LoggerParams.None, "Pong!");
        _loggingHelper.AddInfosForLogging(HttpContext, typeof(object), response);

        return Ok(response);
    }


    [HttpGet($"{AuthEndpoint.TwoFactorAuth}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult> TwoFactorAuthAsync([FromBody] UniqueUserData uniqueUserData)
    {
        _logger.LogDebug(LoggerParams.None, "Initiating two factor authentication!");

        ApiResponse<object> response = new()
        { 
            IsSuccess = await authService.SendVerificationEmailAsync(uniqueUserData)
        };
        
        _loggingHelper.AddInfosForLogging(HttpContext, typeof(object), response);

        return Ok(response);
    }

    [HttpPost($"{AuthEndpoint.CheckUserUniqueness}")]
    [ProducesResponseType(typeof(ApiResponse<ValueTuple<bool, bool>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> CheckUserUniquenessAsync([FromBody] UniqueUserData uniqueUserData)
    {
        _logger.LogDebug(LoggerParams.None, "Checking uniqueness!");

        ApiResponse<(bool email, bool username)> response 
            = await authService.CheckEmailAndUsernameAsync(uniqueUserData);

        _loggingHelper.AddInfosForLogging(HttpContext, typeof(ValueTuple<bool, bool>), response);

        return Ok(response);
    }
}
