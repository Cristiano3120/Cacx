using DotNetEnv;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CacxShared.Helper;

public static class SharedCryptographyHelper
{
    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA384;
    private static readonly byte[] _aesKey; // 32 bytes for AES-256
    private static readonly byte[] _aesIv;  // 16 bytes for AES CBC
    private const int Iterations = 100_000;
    private const int OutputSize = 48;
    private const int SaltLength = 20;

    static SharedCryptographyHelper()
    {
        //Fill with 0 or cut of the rest if the key length is not fitting
        const byte AesKeySize = 32;
        const byte AesIVSize = 16;

        _ = Env.Load(SharedHelper.GetDynamicPath(Project.CacxShared, ".env"));
        _aesKey = Encoding.UTF8.GetBytes(Env.GetString("AES_KEY").PadRight(AesKeySize, '0')[..AesKeySize] 
            ?? throw new JsonException("Json file corrupted"));
        _aesIv = Encoding.UTF8.GetBytes("hbH71!?.GGHm12l==,awe3334")[..AesIVSize];
    }

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

            byte[] bytes = Encrypt("Test!");
            _ = Decrypt(bytes);

            stopwatch.Stop();
            totalElapsedMs += stopwatch.Elapsed.TotalMilliseconds;
        }

        stopwatchTotal.Stop();

        double avgTimeMs = totalElapsedMs / Runs;

        Console.WriteLine($"Cryptography warmup finished. Total time: {stopwatchTotal.Elapsed.TotalMilliseconds:F3}ms");
        Console.WriteLine($"Average time per run: {avgTimeMs:F3}ms");

        await Task.Delay(200);
    }

    public static byte[] Hash(string textToHash)
        => Hash(Encoding.UTF8.GetBytes(textToHash));

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

    /// <summary>
    /// Encrypts a string using AES-256 CBC.
    /// </summary>
    public static byte[] Encrypt(string input)
    {
        using Aes aes = Aes.Create();
        aes.Key = _aesKey;
        aes.IV = _aesIv;

        using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        byte[] buffer = Encoding.UTF8.GetBytes(input);
        byte[] encrypted = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);

        return encrypted;
    }

    /// <summary>
    /// Decrypts a string using AES-256 CBC.
    /// </summary>
    public static string Decrypt(byte[] buffer)
    {
        using Aes aes = Aes.Create();
        aes.Key = _aesKey;
        aes.IV = _aesIv;

        using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        byte[] decrypted = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);

        return Encoding.UTF8.GetString(decrypted);
    }
}
