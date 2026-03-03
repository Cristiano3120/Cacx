namespace CacxShared.Abstractions;

public sealed record User
{
    public long Id { get; init; }
    public string Email { get; init; } = default!;
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string DisplayName { get; init; } = default!;
}