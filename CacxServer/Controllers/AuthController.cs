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
            string msg = result.Error!.Value == RegisterError.EmailOrUsernameTaken
                ? "The email and/or username you entered is already in use"
                : "The email/username you entered is reserved. It might be free in 15min or less";

            return Conflict(new ApiResponse<string>
            {
                IsSuccess = false,
                ApiError = new ApiError
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = msg
                }
            });
        }

        return Created(string.Empty, new ApiResponse<string>
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
