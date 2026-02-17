namespace CacxServer.Abstractions.Auth.Verification;

public enum VerificationError : byte
{
    None = 0,
    RedisUnavailable = 1,
    UnknownError = 2,
    TooManyAttempts = 3,
}
