namespace CacxServer.RateLimiter.AuthRateLimiter.Keys;

public static class RequestEmailKeys
{
    public static string DeviceID(string deviceIDHash)
        => $"rl:verification:resend-email:dev:{deviceIDHash}";
}
