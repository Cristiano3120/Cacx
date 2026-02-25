namespace CacxServer.Abstractions;

public interface ISnowflakeGenerator
{
    public long GenerateId();
}