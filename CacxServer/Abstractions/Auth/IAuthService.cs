using CacxServer.Abstractions.Auth.Register;
using CacxServer.Abstractions.Auth.Verification;
using CacxShared.Abstractions;

namespace CacxServer.Abstractions.Auth;

public interface IAuthService
{
    Task<VerificationResult> VerifyAsync(string authToken, int code);
    Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest);
    Task<bool> ResendVerificationEmailAsync(string authToken);
}