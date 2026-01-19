using CacxClient.Abstractions;
using CacxClient.Abstractions.Auth;
using CacxShared;
using CacxShared.Abstractions;
using Cristiano3120.Logging;

namespace CacxClient.Services;

public class AuthService(IRequestRateLimiter requestRateLimiter, IHttp http, Logger logger) : IAuthService
{
    public async Task<LoginResult> LoginAsync(LoginRequest loginRequest)
    {
        logger.LogInformation(LoggerParams.None, () => "Trying to login");
        if (requestRateLimiter.CheckIfRequestTypeIsRateLimited(RequestType.Login))
        {
            return new LoginResult() 
            {
                ErrorMessage = "Wohhhh chill. Wait a few" //TODO: Via localizationManager
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
        if (requestRateLimiter.CheckIfRequestTypeIsRateLimited(RequestType.Login))
        {
            return new RegisterResult()
            {
                ErrorMessage = "Wohhhh chill. Wait a few" //TODO: Via localizationManager
            };
        }

        ApiResponse<string> apiResponse = await http.PostAsync<RegisterRequest, string>(
            data: registerRequest,
            endpoint: Endpoints.AuthEndpoints.RegisterEndpoint,
            callerInfos: CallerInfos.Create());

        return new RegisterResult()
        {
            Token = apiResponse.Data,
            ErrorMessage = apiResponse?.ApiError?.Message,
        };
    }
}
