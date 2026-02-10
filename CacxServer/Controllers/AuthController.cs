using CacxServer.Abstractions;
using CacxServer.Abstractions.Auth;
using CacxServer.Abstractions.Auth.Register;
using CacxServer.Abstractions.Auth.Verification;
using CacxServer.RateLimiter.AuthRateLimiter.Abstractions;
using CacxShared;
using CacxShared.Abstractions;
using Cristiano3120.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

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
    public async Task<IActionResult> RegisterAsync(
        [FromHeader(Name = AuthHeaderNames.DeviceIdHeader)] string deviceID, 
        [FromBody] RegisterRequest registerRequest)
    {
        logger.LogInformation(LoggerParams.None, () => "Register endpoint called");

        ClientSecurityContext clientSecurityContext = GetClientSecurityContext(deviceID);

        AuthRateLimitResult rateLimitResult = await authRateLimiter.CheckRegisterAsync(clientSecurityContext);
        if (rateLimitResult.IsLimited)
        {
            Response.Headers.RetryAfter = new RetryConditionHeaderValue(rateLimitResult.RetryAfter).ToString();
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

    [HttpGet(Endpoints.AuthEndpoints.RequestVerificationEmail)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ResendVerificationEmailAsync(
        [FromHeader(Name = AuthHeaderNames.AuthTokenHeader)] string authToken,
        [FromHeader(Name = AuthHeaderNames.DeviceIdHeader)] string deviceID)
    {
        logger.LogInformation(LoggerParams.None, () => "ResendVerificationEmail Endpoint called");

        ClientSecurityContext clientSecurityContext = GetClientSecurityContext(deviceID);
        AuthRateLimitResult rateLimitResult = await authRateLimiter.CheckResendVerificationEmailAsync(clientSecurityContext);
        
        if (rateLimitResult.IsLimited)
        {
            Response.Headers.RetryAfter = new RetryConditionHeaderValue(rateLimitResult.RetryAfter).ToString();
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

        ObjectResult invalidSessionResult = StatusCode((int)HttpStatusCode.Unauthorized, new ApiResponse<object>()
        {
            IsSuccess = false,
            ApiError = new ApiError()
            {
                Message = "Session Invalid. RESTART!",
                StatusCode = HttpStatusCode.Unauthorized
            }
        });

        if (string.IsNullOrWhiteSpace(authToken))
        {
            return invalidSessionResult;
        }

        bool success = await authService.ResendVerificationEmailAsync(authToken);
        if (!success)
        {
            return invalidSessionResult;
        }

        return Ok(ApiResponse<bool>.Ok(data: success));
    }

    [ProducesResponseType(typeof(ApiResponse<VerificationResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyEmailAsync(
        [FromHeader(Name = AuthHeaderNames.AuthTokenHeader)] string authToken,
        [FromHeader(Name = AuthHeaderNames.DeviceIdHeader)] string deviceID, 
        [FromBody] int code)
    {
        logger.LogInformation(LoggerParams.None, () => "VerifyEmail Endpoint called");

        ClientSecurityContext clientSecurityContext = GetClientSecurityContext(deviceID);
        AuthRateLimitResult rateLimitResult = await authRateLimiter.CheckResendVerificationEmailAsync(clientSecurityContext);

        if (rateLimitResult.IsLimited)
        {
            Response.Headers.RetryAfter = new RetryConditionHeaderValue(rateLimitResult.RetryAfter).ToString();
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

        VerificationResult verificationResult = await authService.VerifyAsync(authToken, code);
        return verificationResult.VerificationError switch
        {
            VerificationError.None => Ok(ApiResponse<bool>.Ok(data: default!)),
            
            VerificationError.RedisUnavailable => StatusCode((int)HttpStatusCode.ServiceUnavailable, new ApiResponse<object>()
            {
                IsSuccess = false,
                ApiError = new ApiError()
                {
                    StatusCode = HttpStatusCode.ServiceUnavailable,
                    Message = "Service temporarily unavailable. Try again later"
                }
            }),
            
            // Covers UnknownError and any future errors that might be added without updating this switch
            _ => StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<object>()
            {
                IsSuccess = false,
                ApiError = new ApiError()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Unknown error"
                }
            }),
        };
    }

    [HttpPost(Endpoints.AuthEndpoints.Login)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> LoginAsync(
        [FromHeader(Name = "Device-ID")] string deviceID,
        [FromBody] LoginRequest loginRequest)
    {
        logger.LogInformation(LoggerParams.None, () => "Login endpoint called");

        ClientSecurityContext clientSecurityContext = GetClientSecurityContext(deviceID);

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

    private ClientSecurityContext GetClientSecurityContext(string deviceID)
        => new(ClientIP: HttpContext.Connection.RemoteIpAddress, DeviceID: deviceID);
}