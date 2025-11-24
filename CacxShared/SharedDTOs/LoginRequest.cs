namespace CacxShared.SharedDTOs;
public sealed record class LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
