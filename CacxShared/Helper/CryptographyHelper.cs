using System.Security.Cryptography;

namespace CacxShared.Helper;

public static class CryptographyHelper
{
    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA3_384;
    private const int Iterations = 100_000;
    private const int OutputSize = 48;
    private const int SaltLength = 20;

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
