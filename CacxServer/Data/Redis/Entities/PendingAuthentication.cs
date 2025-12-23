namespace CacxServer.Data.Redis.Entities;

public sealed record PendingAuthentication
{
    public required string Email { get; init; } 
    public required string Username { get; init; } 
    public required string VerificationCode { get; init; }
    public byte Attempts { get; set; }
}
