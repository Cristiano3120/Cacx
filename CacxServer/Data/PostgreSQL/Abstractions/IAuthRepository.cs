namespace CacxServer.Data.PostgreSQL.Abstractions;

public interface IAuthRepository
{
    Task<bool> CheckIfUniqueDataExistsAsync(string email, string username);
}
