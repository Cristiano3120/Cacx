using CacxClient.Abstractions;
using CacxShared.Abstractions;
using CacxShared.SharedDTOs;

namespace CacxClient.Services;

public class AuthService : IAuthService
{
    public async Task LoginAsync(LoginRequest loginRequest)
    {
        await Task.Delay(3000);
    }

    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        await Task.Delay(3000);
    }
}
