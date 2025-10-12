namespace CacxShared.SharedDTOs;

public sealed record User
{
    public ulong Id { get; init; }
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public DateOnly Birthday { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Biography { get; init; } = string.Empty;
}
