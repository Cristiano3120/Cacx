using CacxServer.Abstractions.Auth;
using CacxShared;
using CacxShared.Abstractions;
using CacxShared.APIResponse;
using Cristiano3120.Logging;
using Microsoft.AspNetCore.Mvc;

namespace CacxServer.Controllers;

[ApiController]
[Route($"{Endpoints.Base}/{Endpoints.AuthEndpoints.BaseAuth}")]
public class AuthController(IAuthService authService, Logger logger) : ControllerBase
{
    [HttpPost(Endpoints.AuthEndpoints.Register)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest registerRequest)
    {
        logger.LogInformation(LoggerParams.None, () => "Register endpoint called");

        await authService.RegisterAsync(registerRequest);

        return Ok(new ApiResponse<object>() { IsSuccess = true, Data = null});
    }

    [HttpPost(Endpoints.AuthEndpoints.Login)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult Login()
    {
        logger.LogInformation(LoggerParams.None, () => "Login endpoint called");
        return Ok(new ApiResponse<object>() { IsSuccess = true, Data = null });
    }
}
