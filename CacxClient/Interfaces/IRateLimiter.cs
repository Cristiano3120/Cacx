namespace CacxClient.Interfaces;

public interface IRateLimiter
{
    public bool TryConsume();
}
