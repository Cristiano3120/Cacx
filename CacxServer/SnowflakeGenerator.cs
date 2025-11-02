namespace CacxServer;

//[ 1 Bit | 41 Bits Timestamp | 10 Bits Worker ID | 12 Bits Increment ]
public static class SnowflakeGenerator
{
    private readonly static Lock _lock = new();
    private static ulong _lastTimestamp;
    private static long _cacxEpochUnix;
    private static ushort _workerId;
    private static uint _increment;

    public static void Initialize(ushort workerId)
    {
        DateTimeOffset cacxEpoch = new(year: 2025, month: 10, day: 23, hour: 14, minute: 0, second: 0, offset: TimeSpan.Zero);
        _cacxEpochUnix = cacxEpoch.ToUnixTimeMilliseconds();
        _workerId = workerId;
    }

    public static ulong Generate()
    {
        lock (_lock)
        {
            ulong timestamp = (ulong)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _cacxEpochUnix);

            if (timestamp == _lastTimestamp)
            {
                _increment = (_increment + 1) & 0xFFF; // 12 Bit
                if (_increment == 0)
                {
                    while ((ulong)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _cacxEpochUnix) <= timestamp)
                    {
                        // spin wait
                    }

                    timestamp = (ulong)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _cacxEpochUnix);
                }
            }
            else
            {
                _increment = 0;
            }

            _lastTimestamp = timestamp;
            //First bit always 0 cause timestamp is a ulong
            return (timestamp << 22) | ((ulong)_workerId << 12) | (ulong)_increment;
        }
    }
}
