using CacxServer.Data.PostgreSQL.Abstractions;
using Microsoft.EntityFrameworkCore;
using CacxShared.Abstractions;
using CacxServer.Data.PostgreSQL.Entities;

namespace CacxServer.Data.PostgreSQL.Repositories;

public class AuthRepository(CacxDbContext db) : IAuthRepository
{
    public async Task<bool> CheckIfUniqueDataExistsAsync(string email, string username)
        => await db.Users.AnyAsync(x => x.Email == email || x.Username == username);

    public async Task<bool> AddUserAsync(User user, string refreshToken, long userId)
    {
        UserEntity userEntity = new()
        {
            Id = userId,
            Email = user.Email,
            Username = user.Username,
            PasswordHash = user.Password,
            DisplayName = user.DisplayName
        };

        UserJwtEntity userJwtEntity = new()
        {
            UserId = userId,
            RefreshToken = refreshToken
        };

        _ = await db.UsersJwts.AddAsync(userJwtEntity);
        _ = await db.Users.AddAsync(userEntity);

        return await db.SaveChangesAsync() > 0; //Returns true if the user was added successfully, false otherwise
    }
}