using CacxServer.RateLimiter.AuthRateLimiter.Abstractions;
using CacxServer.RateLimiter.AuthRateLimiter.Keys;

namespace CacxServer.RateLimiter.AuthRateLimiter.Services;

public static class RateLimitRuleBuilder
{
    public static IEnumerable<RateLimitRule> BuildRegisterRules(string ipHash, string deviceHash)
    {
        yield return new RateLimitRule(
            Key: RegisterKeys.Ip(ipHash),
            Limit: 10,
            Ttl: TimeSpan.FromMinutes(30));

        yield return new RateLimitRule(
            Key: RegisterKeys.Device(deviceHash),
            Limit: 3,
            Ttl: TimeSpan.FromMinutes(30));

        yield return new RateLimitRule(
            Key: RegisterKeys.IpDevice(ipHash, deviceHash),
            Limit: 2,
            Ttl: TimeSpan.FromMinutes(30));
    }

    public static IEnumerable<RateLimitRule> BuildLoginRules(string ipHash, string deviceHash, string username)
    {
        yield return new RateLimitRule(
            Key: LoginKeys.Ip(ipHash),
            Limit: 25,
            Ttl: TimeSpan.FromMinutes(10)
        );

        yield return new RateLimitRule(
            Key: LoginKeys.Username(username),
            Limit: 5,
            Ttl: TimeSpan.FromMinutes(10)
        );

        yield return new RateLimitRule(
            Key: LoginKeys.DeviceId(deviceHash),
            Limit: 10,
            Ttl: TimeSpan.FromMinutes(10)
        );

        yield return new RateLimitRule(
            Key: LoginKeys.DeviceIdUsername(deviceHash, username),
            Limit: 5,
            Ttl: TimeSpan.FromMinutes(15)
        );

        yield return new RateLimitRule(
            Key: LoginKeys.IpUsername(ipHash, username),
            Limit: 3,
            Ttl: TimeSpan.FromMinutes(5)
        );
    }
}
