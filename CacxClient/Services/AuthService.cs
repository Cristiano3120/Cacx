using CacxClient.Interfaces;
using CacxShared.SharedDTOs;

namespace CacxClient.Services;

public class AuthService : IAuthService
{
    public async Task LoginAsync(LoginRequest loginRequest)
    {
        await Task.Delay(3000);
    }
}
