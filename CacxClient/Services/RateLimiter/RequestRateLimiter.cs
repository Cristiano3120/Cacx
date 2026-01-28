using CacxClient.Abstractions;

namespace CacxClient.Services.RateLimiter;

internal sealed class RequestRateLimiter : IRequestRateLimiter
{
    private readonly Dictionary<RequestType, DateTimeOffset> _rateLimits = [];

    public void AddRateLimit(RequestType requestType, TimeSpan limitedFor)
        => _rateLimits[requestType] = DateTimeOffset.UtcNow + limitedFor;

    public bool CheckIfRequestTypeIsRateLimited(RequestType requestType)
    {
        if (!_rateLimits.TryGetValue(requestType, out DateTimeOffset dateTimeOffset))
        {
            return false;
        }

        if (DateTimeOffset.UtcNow - dateTimeOffset >= TimeSpan.Zero)
        {
            _ = _rateLimits.Remove(requestType);
            return false;
        }

        return true;
    }
}
