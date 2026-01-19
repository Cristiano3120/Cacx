namespace CacxClient.Abstractions.Auth;

public sealed record LoginResult
{
    public bool IsSuccess => ErrorMessage is not null;
    public string? ErrorMessage { get; init; }
}
