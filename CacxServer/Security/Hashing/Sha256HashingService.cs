using System.Security.Cryptography;
using System.Text;

namespace CacxServer.Security.Hashing;

public class Sha256HashingService : IHashingService
{
    public byte[] Hash(string data)
    { 
        if (data is null)
        {
            return [];
        }
        
        return SHA256.HashData(Encoding.UTF8.GetBytes(data)); 
    }

    public bool Verify(string data, byte[] hash)
    {
        if (data is null || hash is null)
        {
            return false;
        }

        Span<byte> computedHash = Hash(data);
        return CryptographicOperations.FixedTimeEquals(computedHash,hash);
    }
}
