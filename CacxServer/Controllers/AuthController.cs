using CacxShared;
using CacxShared.APIResponse;
using Cristiano3120.Logging;
using Microsoft.AspNetCore.Mvc;

namespace CacxServer.Controllers;

[ApiController]
[Route($"{Endpoints.Base}/{Endpoints.AuthEndpoints.BaseAuth}")]
public class AuthController(Logger logger) : ControllerBase
{
    private readonly Logger _logger = logger;

    [HttpPost(Endpoints.AuthEndpoints.Register)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult Register()
    {
        _logger.LogInformation(LoggerParams.None, () => "Register endpoint called");
        return Ok(new ApiResponse<object>() { IsSuccess = true, Data = null});
    }
}
