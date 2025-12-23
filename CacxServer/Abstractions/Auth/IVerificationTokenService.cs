namespace CacxServer.Abstractions.Auth;

public interface IVerificationTokenService
{
    string GenerateVerificationToken();
    int GenerateVerificationCode();
}
