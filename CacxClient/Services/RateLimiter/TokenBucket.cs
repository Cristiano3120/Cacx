using CacxClient.Interfaces;

namespace CacxClient.Services.RateLimiter;

/// <summary>
/// Represents a rate limiter that uses the token bucket algorithm to control the rate of allowed actions over time.
/// </summary>
/// <remarks>The token bucket allows actions to be performed as long as tokens are available. Tokens are
/// regenerated at the specified interval up to the maximum capacity. This implementation is not thread-safe; external
/// synchronization is required if accessed concurrently.</remarks>
/// <param name="regenerationRate">The time interval required to regenerate a single token in the bucket. Must be a positive duration.</param>
/// <param name="maxTokens">The maximum number of tokens that the bucket can hold. Must be greater than zero.</param>
public sealed class TokenBucket(TimeSpan regenerationRate, int maxTokens) : IRateLimiter
{
    private readonly TimeSpan _regenerationRate = regenerationRate;
    private DateTime _lastRegenerated = DateTime.UtcNow;
    private readonly int _maxTokens = maxTokens;
    private int _tokens = maxTokens;

    public void Refill()
    {
        TimeSpan elapsed = DateTime.UtcNow - _lastRegenerated;

        int tokensToAdd = (int)(elapsed.TotalSeconds / _regenerationRate.TotalSeconds);

        if (tokensToAdd > 0)
        {
            _tokens = Math.Clamp(value: _tokens + tokensToAdd, min: 0, max: _maxTokens);
            _lastRegenerated = _lastRegenerated.AddSeconds(tokensToAdd * _regenerationRate.TotalSeconds);
        }
    }

    /// <summary>
    /// Attempts to consume a token from the bucket if one is available.
    /// </summary>
    /// <remarks>This method refills the token bucket before attempting to consume a token. If no tokens are
    /// available after refilling, the method returns false and does not modify the bucket state.</remarks>
    /// <returns>true if a token was successfully consumed; otherwise, false.</returns>
    public bool TryConsume()
    {
        Refill();
        Console.WriteLine(_tokens);
        if (_tokens == 0)
        {
            return false;
        }

        _tokens--;
        return true;
    }
}
