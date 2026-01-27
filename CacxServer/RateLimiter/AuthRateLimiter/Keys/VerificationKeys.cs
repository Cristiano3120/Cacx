namespace CacxServer.RateLimiter.AuthRateLimiter.Keys;

public static class VerificationKeys
{
    public static string DeviceID(string deviceIDHash)
        => $"rl:verification:resend-email:dev:{deviceIDHash}";
}
