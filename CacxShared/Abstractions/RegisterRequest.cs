namespace CacxShared.Abstractions;
public sealed record RegisterRequest
{
    public required string Email { get; init; } 
    public required string Username { get; init; } 
    public required string Password { get; init; }
    public required string DisplayName { get; init; }
    public required string DeviceId {  get; init; } 
}
