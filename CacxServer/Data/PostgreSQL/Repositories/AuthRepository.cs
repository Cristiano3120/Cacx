using CacxServer.Data.PostgreSQL.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CacxServer.Data.PostgreSQL.Repositories;

public class AuthRepository(CacxDbContext db) : IAuthRepository
{
    public async Task<bool> CheckIfUniqueDataExistsAsync(string email, string username)
        =>  await db.Users.AnyAsync(x => x.Email == email || x.Username == username);

    public async Task AddUser() 
    {
        return;
    }
}
