namespace CacxServer.Data.PostgreSQL.Entities;

public sealed record UserJwtEntity
{
    public long UserId { get; init; }
    public string RefreshToken { get; init; } = default!;
}