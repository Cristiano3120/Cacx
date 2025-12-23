namespace CacxServer.Data.PostgreSQL.Entities;

public sealed record DatabaseUser
{
    public ulong Id { get; init; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required string DisplayName { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
}
