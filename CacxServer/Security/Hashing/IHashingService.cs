namespace CacxServer.Security.Hashing;

public interface IHashingService
{
    byte[] Hash(string data);
    bool Verify(string data, byte[] hash);
}
