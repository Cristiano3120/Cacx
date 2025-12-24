using CacxServer.Abstractions.Auth;
using CacxShared;
using CacxShared.Abstractions;
using CacxShared.APIResponse;
using Cristiano3120.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CacxServer.Controllers;

[ApiController]
[Route($"{Endpoints.Base}/{Endpoints.AuthEndpoints.BaseAuth}")]
public class AuthController(IAuthService authService, Logger logger) : ControllerBase
{
    [HttpPost(Endpoints.AuthEndpoints.Register)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest registerRequest)
    {
        logger.LogInformation(LoggerParams.None, () => "Register endpoint called");

        RegisterResult result = await authService.RegisterAsync(registerRequest);

        if (!result.IsSuccess)
        {
            (HttpStatusCode status, string message) = result.Error!.Value switch
            {
                RegisterError.EmailOrUsernameTaken
                    => (HttpStatusCode.Conflict, "The email and/or username you entered is already in use"),

                RegisterError.PendingReservationExists
                    => (HttpStatusCode.Conflict, "The email/username is currently reserved"),

                RegisterError.ServiceUnavailable
                    => (HttpStatusCode.ServiceUnavailable, "Service temporarily unavailable. Try again later"),

                RegisterError.NotificationFailed
                    => (HttpStatusCode.InternalServerError, "Failed to send verification email"),

                RegisterError.Unknown
                    => (HttpStatusCode.InternalServerError, "Unknown error"),
                
                _ => (HttpStatusCode.InternalServerError, "Unknown error")
            };

            return StatusCode((int)status, new ApiResponse<string>
            {
                IsSuccess = false,
                ApiError = new ApiError
                {
                    StatusCode = status,
                    Message = message
                }
            });
        }

        return Created(
            uri: string.Empty,
            value: new ApiResponse<string>
            {
                IsSuccess = true,
                Data = result.Token
            });
    }

    [HttpPost(Endpoints.AuthEndpoints.Login)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult Login()
    {
        logger.LogInformation(LoggerParams.None, () => "Login endpoint called");
        return Ok(new ApiResponse<object>() { IsSuccess = true, Data = null });
    }
}
