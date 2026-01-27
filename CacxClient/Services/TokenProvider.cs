using CacxClient.Abstractions;

namespace CacxClient.Services;

internal sealed class TokenProvider : ITokenProvider
{
    private string? _token;

    public string? GetToken()
    => _token;
    
    public void SetToken(string? token)
    {
        if (token == null)
        {
            return;               
        }

        _token = token;
    }

    public void ClearToken()
        => _token = null;
}
