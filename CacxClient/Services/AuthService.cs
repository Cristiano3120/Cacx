using CacxClient.Abstractions;
using CacxShared;
using CacxShared.Abstractions;
using CacxShared.APIResponse;
using CacxShared.SharedDTOs;
using Cristiano3120.Logging;

namespace CacxClient.Services;

public class AuthService(Http http, Logger logger) : IAuthService
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
            endpoint: $"{Endpoints.Base}/{Endpoints.AuthEndpoints.BaseAuth}/{Endpoints.AuthEndpoints.Register}",
            callerInfos: CallerInfos.Create());

        //TODO: MAybe EndpointProvider Service
        //TODO: RegisterResult zurückgeben und usen z.B errormsg anzeigen
        //TODO: Abstractions folder aufräumen in Auth folder etc unterteilen
    }
}
