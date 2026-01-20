namespace CacxClient.Abstractions.Auth;

public sealed record LoginResult
{
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    public string? ErrorMessage { get; init; }
}
