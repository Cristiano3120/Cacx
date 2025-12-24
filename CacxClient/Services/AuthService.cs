using CacxClient.Abstractions;
using CacxClient.Abstractions.Auth;
using CacxShared;
using CacxShared.Abstractions;
using CacxShared.APIResponse;
using CacxShared.SharedDTOs;
using Cristiano3120.Logging;

namespace CacxClient.Services;

public class AuthService(IHttp http, Logger logger) : IAuthService
{
    public async Task LoginAsync(LoginRequest loginRequest)
    {
        await Task.Delay(3000);
    }

    public async Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest)
    {
        logger.LogInformation(LoggerParams.None, () => "Trying to register");
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
