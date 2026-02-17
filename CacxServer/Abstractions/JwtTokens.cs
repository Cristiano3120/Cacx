namespace CacxServer.Abstractions;

public sealed record JwtTokens(string refreshToken, string accessToken);