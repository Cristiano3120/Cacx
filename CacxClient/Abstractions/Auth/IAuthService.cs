using CacxShared.Abstractions;

namespace CacxClient.Abstractions.Auth;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(LoginRequest loginRequest);
    Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest);
    Task<RequestVerificationEmailResult> RequestVerificationEmailAsync();
    Task VerifyAsync(int code);
}