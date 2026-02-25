namespace CacxServer.Abstractions;

public sealed record JwtTokens(string RefreshToken, string AccessToken);