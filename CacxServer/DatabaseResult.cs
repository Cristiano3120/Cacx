namespace CacxServer;

public readonly struct DatabaseResult<T>
{
    public bool RequestSuccessful { get; init; }
    public T? ReturnedValue { get; init; }
}
