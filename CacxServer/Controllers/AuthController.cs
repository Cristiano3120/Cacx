using CacxServer.Abstractions;
using CacxServer.Abstractions.Auth;
using CacxServer.Abstractions.Auth.Register;
using CacxServer.RateLimiter.AuthRateLimiter.Abstractions;
using CacxShared;
using CacxShared.Abstractions;
using Cristiano3120.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CacxServer.Controllers;

[ApiController]
[Route($"{Endpoints.Base}/{Endpoints.AuthEndpoints.BaseAuth}")]
public class AuthController(IAuthService authService, IAuthRateLimiter authRateLimiter, Logger logger) : ControllerBase
{
    [HttpPost(Endpoints.AuthEndpoints.Register)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest registerRequest)
    {
        logger.LogInformation(LoggerParams.None, () => "Register endpoint called");

        ClientSecurityContext clientSecurityContext = new
        (
            ClientIP: HttpContext.Connection.RemoteIpAddress,
            DeviceID: registerRequest.DeviceId
        );

        AuthRateLimitResult rateLimitResult = await authRateLimiter.CheckRegisterAsync(clientSecurityContext);
        if (rateLimitResult.IsLimited)
        {
            Response.Headers.RetryAfter = rateLimitResult.RetryAfter.TotalSeconds.ToString();
            return StatusCode((int)HttpStatusCode.TooManyRequests, value: new ApiResponse<string>()
            {
                IsSuccess = false,
                ApiError = new ApiError()
                {
                    StatusCode = HttpStatusCode.TooManyRequests,
                    Message = "Too many requests. Try again later..."
                }
            });
        }

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
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
    {
        logger.LogInformation(LoggerParams.None, () => "Login endpoint called");

        ClientSecurityContext clientSecurityContext = new
        (
            ClientIP: HttpContext.Connection.RemoteIpAddress,
            DeviceID: loginRequest.DeviceId
        );

        AuthRateLimitResult rateLimitResult = await authRateLimiter.CheckRegisterAsync(clientSecurityContext);
        if (rateLimitResult.IsLimited)
        {
            Response.Headers.RetryAfter = rateLimitResult.RetryAfter.TotalSeconds.ToString();
            return StatusCode((int)HttpStatusCode.TooManyRequests, value: new ApiResponse<object>()
            {
                IsSuccess = false,
                ApiError = new ApiError()
                {
                    StatusCode = HttpStatusCode.TooManyRequests,
                    Message = "Too many requests. Try again later..."
                }
            });
        }

        return Ok(new ApiResponse<object>() { IsSuccess = true, Data = null });
    }
}
