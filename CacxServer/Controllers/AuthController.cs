using CacxServer.Helper;
using CacxShared.ApiResources;
using CacxShared.Endpoints;
using Cristiano3120.Logging;
using Microsoft.AspNetCore.Mvc;

namespace CacxServer.Controllers;

[ApiController]
[Route($"{Endpoints.BaseEndpoint}/{Endpoints.BaseAuthEndpoint}")]
public sealed class AuthController(LoggingHelper loggingHelper, Logger logger) : ControllerBase
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
}
