namespace CacxClient.Abstractions;
public interface IDeviceIDProvider
{
    Guid GetDeviceID();
    Guid GenerateDeviceID();
}
