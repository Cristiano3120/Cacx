namespace CacxClient.Abstractions.Auth;

public sealed record RegisterResult
{
    public bool IsSuccess => ErrorMessage is not null;
    public string? Token { get; init; }
    public string? ErrorMessage { get; init; }
}
