namespace CacxServer.Data.Redis.Entities;

public sealed record PendingAuthentication
{
    public required string Email { get; init; } 
    public required string Username { get; init; } 
    public required string Password { get; init; }
    public required string DisplayName { get; init; }
    public required byte[] VerificationCode { get; init; }
    public int Attempts { get; set; }
}