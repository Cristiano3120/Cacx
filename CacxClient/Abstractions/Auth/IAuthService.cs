using CacxShared.Abstractions;
using CacxShared.SharedDTOs;

namespace CacxClient.Abstractions.Auth;

public interface IAuthService
{
    Task LoginAsync(LoginRequest loginRequest);

    Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest);
}
