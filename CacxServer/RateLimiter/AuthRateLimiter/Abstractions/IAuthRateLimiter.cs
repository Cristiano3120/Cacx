using CacxServer.Abstractions.Auth;

namespace CacxServer.RateLimiter.AuthRateLimiter.Abstractions;

public interface IAuthRateLimiter
{
    Task<AuthRateLimitResult> CheckRegisterAsync(ClientSecurityContext securityContext);
    Task<AuthRateLimitResult> CheckLoginAsync(ClientSecurityContext securityContext, string username);
    Task<AuthRateLimitResult> CheckResendVerificationEmailAsync(ClientSecurityContext securityContext);
    Task<AuthRateLimitResult> CheckVerifyCodeAsync(ClientSecurityContext securityContext);
}