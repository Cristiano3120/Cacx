using CacxServer.Abstractions.Auth;
using System.Security.Cryptography;

namespace CacxServer.Services;

public sealed class VerificationTokenService : IVerificationTokenService
{
    public string GenerateVerificationToken()
    {
        Span<byte> span = stackalloc byte[32];
        RandomNumberGenerator.Fill(span);

        return Convert.ToBase64String(span);
    }

    public int GenerateVerificationCode()
        => RandomNumberGenerator.GetInt32(100_000, 999_999);
}
