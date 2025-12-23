namespace CacxServer.Abstractions.Auth;

public enum RegisterError : byte
{
    PendingReservationExists,
    EmailOrUsernameTaken
}
