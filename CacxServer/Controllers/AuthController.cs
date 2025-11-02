using CacxServer.Helper;
using CacxServer.Services;
using CacxServer.Storage;
using CacxShared.ApiResources;
using CacxShared.Endpoints;
using CacxShared.SharedDTOs;
using Cristiano3120.Logging;
using Microsoft.AspNetCore.Mvc;

namespace CacxServer.Controllers;

[ApiController]
[Route($"{Endpoints.BaseEndpoint}/{Endpoints.BaseAuthEndpoint}")]
public sealed class AuthController(AuthService authService
    , ObjectStorageManager storageManager
    , LoggingHelper loggingHelper
    , Logger logger) : ControllerBase
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


    [HttpPost($"{AuthEndpoint.StartTwoFactorAuth}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult> StartTwoFactorAuthAsync([FromBody] UniqueUserData uniqueUserData)
    {
        _logger.LogDebug(LoggerParams.None, "Initiating two factor authentication!");

        int verificationCode = authService.CreateAndSaveCode(uniqueUserData.Username ?? "");
        ApiResponse<object> response = new()
        {
            IsSuccess = await authService.SendVerificationEmailAsync(uniqueUserData, verificationCode)
        };

        _loggingHelper.AddInfosForLogging(HttpContext, typeof(object), response);

        return Ok(response);
    }

    [HttpPost($"{AuthEndpoint.Verify}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public ActionResult CheckVerificationCode([FromBody] VerificationRequestData requestData)
    {
        _logger.LogDebug(LoggerParams.None, $"{requestData.Username} is trying to verify with code {requestData.VerificationCode}!");

        ApiResponse<bool> response = new()
        {
            IsSuccess = true,
            Data = authService.CheckVerificationCode(requestData.Username, requestData.VerificationCode)
        };

        _loggingHelper.AddInfosForLogging(HttpContext, typeof(bool), response);
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

    [HttpPost($"{AuthEndpoint.CreateAcc}")]
    [ProducesResponseType(typeof(ApiResponse<User>), StatusCodes.Status200OK)]
    public async Task<ActionResult> CreateUserAsync([FromBody] User user)
    {
        _logger.LogDebug(LoggerParams.None, "Saving the user to the database!");

        User? createdUser = await authService.SaveUserToDbAsync(user);
        ApiResponse<User> response = new()
        {
            IsSuccess = user is not null,
            Data = createdUser
        };

        _loggingHelper.AddInfosForLogging(HttpContext, typeof(User), response);
        return Ok(response);
    }

    [HttpPost($"{AuthEndpoint.UploadProfilePicture}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult> UploadProfilePictureAsync([FromForm] ProfilePictureUploadRequestServerSide uploadRequest)
    {
        bool successful = await storageManager.UploadProfilePictureAsync(uploadRequest);

        ApiResponse<object> response = new()
        {
            IsSuccess = successful,
        };

        _loggingHelper.AddInfosForLogging(HttpContext, typeof(object), response);
        return Ok(response);
    }
}
