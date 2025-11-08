namespace CacxShared.SharedDTOs;

public sealed record User
{
    public ulong Id { get; init; }
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;

    /// <summary>
    /// Contains only the resource-path part of the profile picture URL.
    /// This means only the {Id}/profilePicture portion.
    /// </summary>
    public string? ProfilePictureUrl { get; init; }
    public byte[] ProfilePictureBytes { get; init; } = [];
    public DateOnly Birthday { get; init; }
    public string Biography { get; init; } = string.Empty;
}
