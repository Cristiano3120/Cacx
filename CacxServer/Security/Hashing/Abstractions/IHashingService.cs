namespace CacxServer.Security.Hashing.Abstractions;

public interface IHashingService
{
    byte[] Hash(string data);
    bool Verify(string data, byte[] hash);
}
