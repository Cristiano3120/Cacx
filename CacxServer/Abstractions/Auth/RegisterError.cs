namespace CacxServer.Abstractions.Auth;

public enum RegisterError : byte
{
    PendingReservationExists,
    EmailOrUsernameTaken,
    ServiceUnavailable,
    NotificationFailed,
    Unknown
}
