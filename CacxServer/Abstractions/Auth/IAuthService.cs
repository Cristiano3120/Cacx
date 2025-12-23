using CacxShared.Abstractions;

namespace CacxServer.Abstractions.Auth;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest);
}
