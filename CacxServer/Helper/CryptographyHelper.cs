using DotNetEnv;
using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using System.Security.Cryptography;
using System.Text;

namespace CacxServer.Helper;

public static class CryptographyHelper
{
    private static readonly byte[] _aesKey; // 32 bytes for AES-256
    private static readonly byte[] _aesIv;  // 16 bytes for AES CBC

    static CryptographyHelper()
    {
        //Fill with 0 or cut of the rest if the key length is not fitting
        const byte AesKeySize = 32;
        const byte AesIVSize = 16;

        _aesKey = Encoding.UTF8.GetBytes(Env.GetString("AES_KEY").PadRight(AesKeySize, '0')[..AesKeySize]); 
        _aesIv = Encoding.UTF8.GetBytes("hbH71!?.GGHm12l==,awe3334")[..AesIVSize];

        Warmup();
    }

    /// <summary>
    /// Prepares cryptographic operations to avoid first-run latency.
    /// </summary>
    public static void Warmup()
    {
        //TODO: Improve warmup
        // Dummy hash
        _ = HashPassword("warmup");

        // Dummy AES encrypt
        _ = Encrypt("warmup");
    }

    /// <summary>
    /// Hashes a password using Argon2id.
    /// </summary>
    public static byte[] HashPassword(string password)
    {
        Argon2Config cfg = new()
        {
            MemoryCost = 1024 * 32, // 32 MB RAM
            Threads = 4,
            Salt = RandomNumberGenerator.GetBytes(16),
            Password = Encoding.UTF8.GetBytes(password),
        };

        using Argon2 argon2 = new(cfg);
        SecureArray<byte> secureHash = argon2.Hash();

        byte[] hashBytes = secureHash.Buffer;
        secureHash.Dispose();

        return hashBytes;
    }

    /// <summary>
    /// Verifies a password against an Argon2 hash.
    /// </summary>
    public static bool VerifyHash(string input, byte[] hash)
    {
        string hashStr = Encoding.UTF8.GetString(hash);
        return Argon2.Verify(hashStr, input);
    }

    /// <summary>
    /// Encrypts a string using AES-256 CBC.
    /// </summary>
    public static string Encrypt(string input)
    {
        using Aes aes = Aes.Create();
        aes.Key = _aesKey;
        aes.IV = _aesIv;

        using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        byte[] buffer = Encoding.UTF8.GetBytes(input);
        byte[] encrypted = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);

        return Convert.ToBase64String(encrypted);
    }

    /// <summary>
    /// Decrypts a string using AES-256 CBC.
    /// </summary>
    public static string Decrypt(string base64Input)
    {
        using Aes aes = Aes.Create();
        aes.Key = _aesKey;
        aes.IV = _aesIv;

        using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        byte[] buffer = Convert.FromBase64String(base64Input);
        byte[] decrypted = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);

        return Encoding.UTF8.GetString(decrypted);
    }
}
