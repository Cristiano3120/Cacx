namespace CacxServer.Abstractions.Auth;

public enum ResendVerificationResult : byte 
{
    Success,
    SessionInvalid,
    EmailSendFailed
}