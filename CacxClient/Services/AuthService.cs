using Cacx.LocalizationManager.Abstractions;
using CacxClient.Abstractions;
using CacxClient.Abstractions.Auth;
using CacxClient.Resources;
using CacxShared;
using CacxShared.Abstractions;
using Cristiano3120.Logging;

namespace CacxClient.Services;

public sealed class AuthService(ILocalizationProvider localizationProvider, IRequestRateLimiter requestRateLimiter, IHttp http, Logger logger) : IAuthService
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
            requestRateLimiter.AddRateLimit(RequestType.Register, limitedTill: DateTimeOffset.UtcNow + retryAfter);
        }

        return new RegisterResult()
        {
            Token = apiResponse.Data,
            ErrorMessage = apiResponse?.ApiError?.Message,
        };
    }
}
