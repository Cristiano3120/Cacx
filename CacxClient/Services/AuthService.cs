using Cacx.LocalizationManager.Abstractions;
using CacxClient.Abstractions;
using CacxClient.Abstractions.Auth;
using CacxClient.Resources;
using CacxShared;
using CacxShared.Abstractions;
using Cristiano3120.Logging;
using System.Net;

namespace CacxClient.Services;

public sealed class AuthService(
    ILocalizationProvider localizationProvider,
    IRequestRateLimiter requestRateLimiter,
    IDeviceIDProvider deviceIDProvider,
    ITokenProvider tokenProvider,
    IHttp http,
    Logger logger) : IAuthService
{
    public async Task<LoginResult> LoginAsync(LoginRequest loginRequest)
    {
        logger.LogInformation(LoggerParams.None, () => "Trying to login");
        if (requestRateLimiter.CheckIfRequestTypeIsRateLimited(RequestType.Login))
        {
            localizationProvider.UpdateContext(ResourceBasePaths.GeneralAuth);
            return new LoginResult()
            {
                ErrorMessage = localizationProvider.GetString(key: "OnCooldownMessage")
            };
        }

        ApiResponse<object> apiResponse = await http.PostAsync<int, object>(
            data: 1,
            endpoint: Endpoints.AuthEndpoints.LoginEndpoint,
            callerInfos: CallerInfos.Create());

        return new LoginResult()
        {

        };

        //TODO: Display info return LoginResult
    }

    public async Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest)
    {
        logger.LogInformation(LoggerParams.None, () => "Trying to register");
        if (requestRateLimiter.CheckIfRequestTypeIsRateLimited(RequestType.Register))
        {
            localizationProvider.UpdateContext(ResourceBasePaths.GeneralAuth);
            return new RegisterResult()
            {
                ErrorMessage = localizationProvider.GetString(key: "OnCooldownMessage")
            };
        }

        ApiResponse<string> apiResponse = await http.PostAsync<RegisterRequest, string>(
            data: registerRequest,
            endpoint: Endpoints.AuthEndpoints.RegisterEndpoint,
            callerInfos: CallerInfos.Create());

        if (apiResponse.RetryAfter is TimeSpan retryAfter)
        {
            requestRateLimiter.AddRateLimit(RequestType.Register, limitedFor: retryAfter);
        }

        tokenProvider.SetToken(apiResponse.Data);
        return new RegisterResult()
        {
            Token = apiResponse.Data,
            ErrorMessage = apiResponse?.ApiError?.Message,
        };
    }

    public async Task VerifyAsync(int code)
    {

    }

    public async Task<RequestVerificationEmailResult> RequestVerificationEmailAsync()
    {
        logger.LogInformation(LoggerParams.None, () => "Requesting another verification email");
        if (requestRateLimiter.CheckIfRequestTypeIsRateLimited(RequestType.RequestVerificationEmail))
        {
            localizationProvider.UpdateContext(ResourceBasePaths.GeneralAuth);
            return new RequestVerificationEmailResult()
            {
                ErrorMessage = localizationProvider.GetString(key: "OnCooldownMessage")
            };
        }

        ApiResponse<bool> apiResponse = await http.PostAsync<string, bool>(
            data: deviceIDProvider.GetDeviceID().ToString(),
            endpoint: Endpoints.AuthEndpoints.RequestVerificationEmailEndpoint,
            callerInfos: CallerInfos.Create());

        if (apiResponse.IsSuccess)
        {
            return new RequestVerificationEmailResult(); 
        }

        localizationProvider.UpdateContext(ResourceBasePaths.GeneralAuth);
        return new RequestVerificationEmailResult()
        {
            SessionExpired = apiResponse.ApiError.StatusCode == HttpStatusCode.Unauthorized,
            ErrorMessage = apiResponse.ApiError.StatusCode switch
            {
                HttpStatusCode.Unauthorized => localizationProvider.GetString(key: "SessionExpired"),
                HttpStatusCode.TooManyRequests => localizationProvider.GetString(key: "OnCooldownMessage"),
                _ => apiResponse.ApiError.Message,
            }
        };
    }
}