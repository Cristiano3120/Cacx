using System.Net;

namespace CacxServer.Abstractions.Auth;

public sealed record ClientSecurityContext
{
    public required IPAddress? ClientIP { get; init; }
    public required string DeviceID { get; init; }
}
