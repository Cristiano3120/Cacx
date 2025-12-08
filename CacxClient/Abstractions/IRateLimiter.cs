namespace CacxClient.Abstractions;

public interface IRateLimiter
{
    public bool TryConsume();
}
