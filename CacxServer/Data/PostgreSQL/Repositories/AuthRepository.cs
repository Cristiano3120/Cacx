using CacxServer.Data.PostgreSQL.Abstractions;

namespace CacxServer.Data.PostgreSQL.Repositories;

public class AuthRepository : IAuthRepository
{
    public async Task<bool> CheckIfUniqueDataExistsAsync(string email, string username)
    {
        return true;
    }
}
