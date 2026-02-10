namespace CacxServer.RateLimiter.AuthRateLimiter.Keys;

public static class VerificationKeys
{
    public static string Ip(string ipHash)
        => $"rl:verification:ip:{ipHash}";

    public static string DeviceID(string deviceIdHash)
        => $"rl:verification:dev:{deviceIdHash}";
}