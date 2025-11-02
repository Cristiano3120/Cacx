using CacxShared.Helper;
using CacxShared.SharedDTOs;

namespace CacxServer.UserDataDatabaseResources;

public sealed record DbUser
{
    public ulong Id { get; init; }
    public string Username { get; init; } = default!;
    public byte[] Email { get; init; } = default!;
    public byte[] EmailHash { get; init; } = default!;
    public byte[] PasswordHash { get; init; } = default!;
    public byte[] FirstName { get; init; } = default!;
    public byte[] LastName { get; init; } = default!;
    public string? ProfilePictureUrl { get; init; } = default!;
    public DateOnly Birthday { get; init; }
    public string Biography { get; init; } = string.Empty;

    public DbUser() { }

    public DbUser(User user)
    {
        Id = user.Id;
        Username = user.Username;
        Email = SharedCryptographyHelper.Encrypt(user.Email);
        EmailHash = SharedCryptographyHelper.Hash(user.Email);
        PasswordHash = SharedCryptographyHelper.Hash(user.Password);
        FirstName = SharedCryptographyHelper.Encrypt(user.FirstName);
        LastName = SharedCryptographyHelper.Encrypt(user.LastName);
        ProfilePictureUrl = user.ProfilePictureUrl;
        Birthday = user.Birthday;
        Biography = user.Biography;
    }
}
