using CacxShared.ApiResources;
using CacxShared.Endpoints;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CacxServer.Controllers;

[ApiController]
[Route($"{Endpoints.BaseEndpoint}/{Endpoints.BaseAuthEndpoint}")]
public sealed class AuthController : ControllerBase
{
    private JsonSerializerOptions _jsonSerializerOptions;
    public AuthController(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    [HttpGet($"{AuthEndpoint.Ping}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public ActionResult Ping()
    {
        ApiResponse<bool> response = new(true, true);
        AddInfosForLogging(typeof(bool), response);

        return Ok(response);
    }


    [HttpPost($"test")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public ActionResult Test([FromBody] int zahl)
    {
        ApiResponse<int> response = new(true, 11);
        AddInfosForLogging(typeof(bool), response);
        return Ok(response); 

    }

    /// <summary>
    /// Adds the return type to the HttpContext so it can be read later for logging purposes
    /// </summary>
    private void AddInfosForLogging<T>(Type type, ApiResponse<T> apiResponse)
    { 
        HttpContext.Items.Add("Type", type);
        HttpContext.Items.Add("ApiResponseStr", JsonSerializer.Serialize(apiResponse, _jsonSerializerOptions));
    }
}
