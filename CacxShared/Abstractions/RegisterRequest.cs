namespace CacxShared.Abstractions;
public sealed record RegisterRequest
{
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
}
