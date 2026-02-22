using BenchmarkDotNet.Attributes;
using Microsoft.VSDiagnostics;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Benchmark;
// For more information on the VS BenchmarkDotNet Diagnosers see https://learn.microsoft.com/visualstudio/profiling/profiling-with-benchmark-dotnet
public class Benchmarks
{
    [Benchmark]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetCurrentTimestamp()
        => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    [Benchmark]
    public long GetCurrentTimestampNoInline()
        => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
