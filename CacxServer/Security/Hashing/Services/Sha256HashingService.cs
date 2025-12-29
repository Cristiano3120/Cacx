using CacxServer.Security.Hashing.Abstractions;
using System.Security.Cryptography;
using System.Text;
using DotNetEnv;

namespace CacxServer.Security.Hashing.Services;

public class Sha256HashingService : IHashingService
{
    private readonly byte[] _salt = Encoding.UTF8.GetBytes(Env.GetString(key: "AUTH_RATE_LIMIT_SALT"));

    public byte[] Hash(string data)
    { 
        if (data is null)
        {
            return [];
        }

        using HMACSHA256 hmac = new(key: _salt);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    public bool Verify(string data, byte[] hash)
    {
        if (data is null || hash is null)
        {
            return false;
        }

        Span<byte> computedHash = Hash(data);
        return CryptographicOperations.FixedTimeEquals(computedHash, hash);
    }
}
