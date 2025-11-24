using CacxShared.SharedDTOs;

namespace CacxClient.Interfaces;

public interface IAuthService
{
    public Task LoginAsync(LoginRequest loginRequest);
}
