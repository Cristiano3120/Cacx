using System.Net;

namespace CacxServer.Abstractions.Auth;

public sealed record ClientSecurityContext(IPAddress? ClientIP, string DeviceID);