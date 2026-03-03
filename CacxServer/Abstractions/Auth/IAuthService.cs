using CacxServer.Abstractions.Auth.Register;
using CacxServer.Abstractions.Auth.Verification;
using CacxShared.Abstractions;

namespace CacxServer.Abstractions.Auth;

public interface IAuthService
{
    Task<VerificationResult?> VerifyAsync(string authToken, string deviceID, int code);
    Task<ResendVerificationResult> ResendVerificationEmailAsync(string authToken);
    Task<RegisterResult> RegisterAsync(RegisterRequest registerRequest);
}