using System.Text;

namespace CacxServer.Security.Hashing;

public class BCryptHashingService : IHashingService
{
    public byte[] Hash(string data)
    {
        if (data is null)
        {
            return [];
        }

        return Encoding.UTF8.GetBytes(BCrypt.Net.BCrypt.HashString(data));
    }

    public bool Verify(string data, byte[] hash)
        => BCrypt.Net.BCrypt.Verify(data, Encoding.UTF8.GetString(hash));

}
