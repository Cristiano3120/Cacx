namespace CacxServer.RateLimiter.AuthRateLimiter.Keys;

public static class LoginKeys
{
    public static string Username(string usernameHash)
        => $"rl:login:user:{usernameHash}";

    public static string Ip(string ipHash)
        => $"rl:login:ip:{ipHash}";

    public static string DeviceID(string deviceIdHash)
        => $"rl:login:dev:{deviceIdHash}";

    public static string IpUsername(string ipHash, string usernameHash)
        => $"rl:login:ip_user:{ipHash}:{usernameHash}";

    public static string DeviceIdUsername(string deviceIdHash, string usernameHash)
        => $"rl:login:dev_user:{deviceIdHash}:{usernameHash}";
}