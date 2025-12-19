using CacxClient.Abstractions;
using CacxShared.SharedDTOs;

namespace CacxClient.Services;

public class AuthService : IAuthService
{
    public async Task LoginAsync(LoginRequest loginRequest)
    {
        await Task.Delay(3000);
    }

    public async Task RegisterAsync(object registerRequest)
    {
        await Task.Delay(3000);
    }
}
