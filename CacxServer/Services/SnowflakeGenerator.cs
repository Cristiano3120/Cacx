using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CacxServer.Services;

public sealed class SnowflakeGenerator
{
    private const int SequenceBits = 12;
    private const int WorkerBits = 10;

    private const long SequenceMask = (1L << SequenceBits) - 1; // Max: 4095
    private const long WorkerMask = (1L << WorkerBits) - 1; // Max: 1023

    private readonly long _workerId;
    private long _lastTimestamp;
    private long _sequence = 0;

    private readonly object _lock = new();
    private readonly long _cacxEpoch = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
    public SnowflakeGenerator(long workerId)
    {
        if (workerId is < 0 or > WorkerMask)
        {
            ushort maxWorkerValue = CalculateMaxValue(bits: WorkerBits);
            throw new ArgumentOutOfRangeException(nameof(workerId), $"Worker ID must be between 0 and {maxWorkerValue}.");
        }

        _workerId = workerId;
    }

    public long GenerateId()
    {
        lock (_lock) 
        {
            long timestamp = GetCurrentTimestamp();

            if (timestamp < _lastTimestamp)
                throw new InvalidOperationException("Clock moved backwards.");

            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0)
                {
                    timestamp = WaitNextMillis(timestamp);
                }
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;


            return ((timestamp - _cacxEpoch) << (WorkerBits + SequenceBits))
                   | (_workerId << SequenceBits)
                   | _sequence;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long GetCurrentTimestamp()
        => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static long WaitNextMillis(long lastTimestamp)
    {
        SpinWait spin = new();
        long currentTimespan;

        do
        {
            spin.SpinOnce();
            currentTimespan = GetCurrentTimestamp();
        }
        while (currentTimespan <= lastTimestamp);

        return currentTimespan;
    }

    /// <summary>
    /// 1 &lt;&lt; bits is equivalent to 2 ^ bits,
    /// and subtracting 1 gives us the maximum value that can be represented with the specified number of bits.
    /// </summary>
    /// <param name = "bits" > The size in bits of the value</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ushort CalculateMaxValue(byte bits)
        => (ushort)((1 << bits) - 1);

    /// <summary>
    /// [BENCHMARK]
    /// TEST METHOD: TESTS THE OUTPUT OF THE ID GENERATOR BY CREATING A SPECIFIED NUMBER OF IDS IN PARALLEL AND COUNTING HOW MANY IDS ARE GENERATED PER MILLISECOND.
    /// </summary>
    /// <param name="iterations"></param>
    /// <returns></returns>
    public int TestGeneratorOutput(int iterations)
    {
        ConcurrentBag<long> ids = new ConcurrentBag<long>();

        // IDs parallel erzeugen
        Parallel.For(0, iterations, i =>
        {
            long id = GenerateId();
            ids.Add(id);
        });

        ConcurrentDictionary<long, int> perMs = new();

        foreach (long id in ids)
        {
            long timestamp = (id >> 22);     
            long msTime = timestamp + _cacxEpoch;

            _ = perMs.AddOrUpdate(msTime, 1, (_, old) => old + 1);
        }

        return perMs.Values.Max();   
    }
}