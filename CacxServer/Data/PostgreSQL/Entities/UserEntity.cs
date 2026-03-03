namespace CacxServer.Data.PostgreSQL.Entities;

public sealed record UserEntity
{
    public long Id { get; init; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required string DisplayName { get; set; }
}