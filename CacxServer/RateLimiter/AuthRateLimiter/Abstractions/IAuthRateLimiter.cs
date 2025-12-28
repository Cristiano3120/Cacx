using CacxServer.Abstractions.Auth;

namespace CacxServer.RateLimiter.AuthRateLimiter.Abstractions;

public interface IAuthRateLimiter
{
    Task<bool> CheckRegisterAsync(ClientSecurityContext securityContext);
    Task<bool> CheckLoginAsync(ClientSecurityContext securityContext);
}
