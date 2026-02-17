namespace CacxServer.Services;

public sealed class SnowflakeGenerator
{
    private const long CacxEpoch = 1771318359; // 17.02.2026 9:53 UTC in seconds since Unix epoch

    //10 bits
    private const short MaxWorkerId = 1023;
    private const short MinWorkerId = 0;
    private readonly short _machineId;

    //12 bits
    private const short MaxIncrementNum = 4095;
    private short _incrementNum;

    private DateTimeOffset _lastTimestamp;

    private byte _timestampLeftShift = ;

    public SnowflakeGenerator(short machineId)
    {
        if (machineId is < MinWorkerId or > MaxWorkerId)
        {
            throw new ArgumentOutOfRangeException(nameof(machineId), $"Machine ID must be between {MinWorkerId} and {MaxWorkerId}.");
        }

        _machineId = machineId;
    }

    public long GenerateId()
    {
        DateTimeOffset currentTimestamp = DateTimeOffset.UtcNow;
        if (currentTimestamp < _lastTimestamp)
        {
            throw new InvalidOperationException("Clock moved backwards. Refusing to generate id.");
        }

        if (currentTimestamp == _lastTimestamp)
        {
            _incrementNum = (short)((_incrementNum + 1) & MaxIncrementNum);
            if (_incrementNum == 0)
            {
                currentTimestamp =  WaitForNextMillis(_lastTimestamp);
            }
            else
            {
                _incrementNum = 0;
            }
        }

        _lastTimestamp = currentTimestamp;
        return (currentTimestamp - CacxEpoch) << _timestampLeftShift;
    }

    private DateTimeOffset WaitForNextMillis(DateTimeOffset lastTimestamp)
    {
        DateTimeOffset currentTimestamp = DateTimeOffset.UtcNow;
        while (currentTimestamp <= lastTimestamp)
        {
            currentTimestamp = DateTimeOffset.UtcNow;
        }

        return currentTimestamp;
    }
}
