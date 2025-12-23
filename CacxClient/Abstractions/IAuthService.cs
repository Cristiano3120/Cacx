using CacxShared.Abstractions;
using CacxShared.SharedDTOs;

namespace CacxClient.Abstractions;

public interface IAuthService
{
    Task LoginAsync(LoginRequest loginRequest);

    Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest);
}
