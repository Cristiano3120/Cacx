using CacxShared.Abstractions;

namespace CacxClient.Abstractions.Auth;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(LoginRequest loginRequest);
    Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest);
    Task RequestVerificationEmailAsync();
    Task VerifyAsync(int code);
}
