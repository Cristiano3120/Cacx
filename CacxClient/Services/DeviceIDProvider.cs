using CacxClient.Abstractions;
using CacxShared.Abstractions;
using System.IO;
using System.Security.Cryptography;

namespace CacxClient.Services;

internal sealed class DeviceIDProvider(IPathProvider pathProvider) : IDeviceIDProvider
{
    private Guid? _cachedDeviceID;

    public Guid GetDeviceID()
    {
        if (_cachedDeviceID.HasValue)
            return _cachedDeviceID.Value;

        if (!File.Exists(path: pathProvider.GetAppDataPath("deviceID.bin")))
            return GenerateDeviceID();

        byte[] encryptedData = File.ReadAllBytes(path: pathProvider.GetAppDataPath("deviceID.bin"));
        if (encryptedData.Length == 0)
        {
            return GenerateDeviceID();
        }

        byte[] deviceId = ProtectedData.Unprotect
            (encryptedData: encryptedData,
            optionalEntropy: null,
            scope: DataProtectionScope.LocalMachine);

        _cachedDeviceID = new Guid(deviceId);
        return _cachedDeviceID.Value;
    }

    public Guid GenerateDeviceID()
    {
        byte[] deviceId = Guid.NewGuid().ToByteArray();
        byte[] encryptedData = ProtectedData.Protect
            (userData: deviceId,
            optionalEntropy: null,
            scope: DataProtectionScope.LocalMachine);

        File.WriteAllBytes(path: pathProvider.GetAppDataPath("deviceID.bin"), encryptedData);
        return new Guid(deviceId);
    }
}
