namespace CacxServer.Abstractions.Auth.Register;

public enum RegisterError : byte
{
    PendingReservationExists,
    EmailOrUsernameTaken,
    ServiceUnavailable,
    NotificationFailed,
    Unknown
}
