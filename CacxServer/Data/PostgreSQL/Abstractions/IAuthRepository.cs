using CacxShared.Abstractions;

namespace CacxServer.Data.PostgreSQL.Abstractions;

public interface IAuthRepository
{
    Task<bool> CheckIfUniqueDataExistsAsync(string email, string username);
    Task<bool> AddUserAsync(User user, string refreshToken, long userId);
}