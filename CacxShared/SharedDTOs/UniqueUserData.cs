namespace CacxShared.SharedDTOs;

public sealed record UniqueUserData
{
    public required string Email { get; init; }
    public required string? Username { get; init; }
}
