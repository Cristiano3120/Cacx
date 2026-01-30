namespace CacxClient.Abstractions;

public interface IRequestRateLimiter
{
    bool CheckIfRequestTypeIsRateLimited(RequestType requestType);
    void AddRateLimit(RequestType requestType, TimeSpan limitedFor);
}