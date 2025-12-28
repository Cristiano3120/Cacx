namespace CacxServer.RateLimiter.AuthRateLimiter.Keys;

public static class RegisterKeys
{
    public static string Ip(string ipHash)
        => $"rl:register:ip:{ipHash}";

    public static string Device(string devîceHash)
        => $"rl:register:device:{devîceHash}";

    public static string IpDevice(string ipHash, string deviceHash)
        => $"rl:register:ip_device:{ipHash}:{deviceHash}";
}
