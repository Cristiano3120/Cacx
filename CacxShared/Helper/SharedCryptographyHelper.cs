using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace CacxShared.Helper;

public static class SharedCryptographyHelper
{
    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA384;
    private const int Iterations = 50_000;
    private const int OutputSize = 48;
    private const int SaltLength = 20;

    public static async Task WarmupAsync()
    {
        Console.WriteLine("Starting SharedCryptographyHelper warmup");

        const int Runs = 3;
        Stopwatch stopwatchTotal = Stopwatch.StartNew();
        double totalElapsedMs = 0;

        for (int i = 0; i < Runs; i++)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            byte[] testBytes = Encoding.UTF8.GetBytes("TestBytes!!!");
            _ = Hash(testBytes);
            _ = Verify(testBytes, testBytes);

            stopwatch.Stop();
            totalElapsedMs += stopwatch.Elapsed.TotalMilliseconds;
        }

        stopwatchTotal.Stop();

        double avgTimeMs = totalElapsedMs / Runs;

        Console.WriteLine($"Cryptography warmup finished. Total time: {stopwatchTotal.Elapsed.TotalMilliseconds:F3}ms");
        Console.WriteLine($"Average time per run: {avgTimeMs:F3}ms");

        await Task.Delay(200);
    }

    public static byte[] Hash(byte[] bytesToHash)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltLength);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(bytesToHash, salt, Iterations, _hashAlgorithmName, OutputSize);
        byte[] finalResult = new byte[SaltLength + OutputSize];

        Array.Copy(salt, 0, finalResult, 0, SaltLength);
        Array.Copy(hash, 0, finalResult, SaltLength, OutputSize);

        return finalResult;
    }

    public static bool Verify(byte[] bytesToCheck, byte[] storedHash)
    {
        //Check if the byte[] contains the salt and the hash
        if (storedHash.Length != SaltLength + OutputSize)
            return false;

        byte[] salt = new byte[SaltLength];
        byte[] expectedHash = new byte[OutputSize];

        Array.Copy(storedHash, 0, salt, 0, SaltLength);
        Array.Copy(storedHash, SaltLength, expectedHash, 0, OutputSize);

        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(bytesToCheck, salt, Iterations, _hashAlgorithmName, OutputSize);

        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }
}
