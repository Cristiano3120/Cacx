namespace CacxShared.Abstractions;

public sealed record User
{
    public string? Email { get; init; }
    public long Id { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
    public string? DisplayName { get; init; }
}