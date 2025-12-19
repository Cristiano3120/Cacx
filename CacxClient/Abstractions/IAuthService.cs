using CacxShared.SharedDTOs;

namespace CacxClient.Abstractions;

public interface IAuthService
{
    public Task LoginAsync(LoginRequest loginRequest);

    public Task RegisterAsync(object registerRequest);
}
