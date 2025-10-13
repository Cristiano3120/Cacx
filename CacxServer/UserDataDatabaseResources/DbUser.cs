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
    public string ProfilePictureUrl { get; init; } = default!;
    public DateOnly Birthday { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Biography { get; init; } = string.Empty;
    public bool Verified { get; init; } = false;
}
