using CacxShared;
using CacxShared.SharedDTOs;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CacxClient;

internal static class AutoLogin
{
    private const string SubKeyName = "Cacx";
    private const string KeyName = "Entropy";
    private const string LoginDataFilename = "login.dat";
    private const string SoftwareSubKeyName = "Software";

    public static void AddLoginData(LoginData loginData)
    {
        //Format login data
        string jsonString = JsonSerializer.Serialize(loginData);
        byte[] plaintext = Encoding.UTF8.GetBytes(jsonString);
        
        //Encrypt login data
        byte[] entropy = RandomNumberGenerator.GetBytes(32);
        byte[] ciphertext = ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser);

        //Save the entropy
        RegistryKey cacxSubKey = GetCacxRegistry()
            ?? Registry.CurrentUser.CreateSubKey(SoftwareSubKeyName).CreateSubKey(SubKeyName);

        if (cacxSubKey.GetValue(KeyName) is not null)
        {
            cacxSubKey.DeleteValue(KeyName);
        }
        
        cacxSubKey.SetValue(KeyName, entropy, RegistryValueKind.Binary);
        cacxSubKey.Close();

        //Save the password
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), MessengerInfos.Name);
        _ = Directory.CreateDirectory(appDataPath);
        File.WriteAllBytes(Path.Combine(appDataPath, LoginDataFilename), ciphertext);
    }

    public static void RemoveLoginData()
    {
        //Delete the login data file
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), MessengerInfos.Name);
        File.Delete(Path.Combine(appDataPath, LoginDataFilename));

        //Delete the reg key
        RegistryKey? cacxSubKey = GetCacxRegistry();
        if (cacxSubKey?.GetValue(KeyName) is not null)
        {
            cacxSubKey.DeleteValue(KeyName);
        }
    }

    public static void ChangeLoginData(LoginData loginData)
    {
        RemoveLoginData();
        AddLoginData(loginData);    
    }

    public static LoginData? GetLoginData()
    {
        try
        {
            //Get the entropy from the reg
            RegistryKey? cacxSubKey = GetCacxRegistry();
            if (cacxSubKey is null)
                return null;

            byte[]? entropy = (byte[])cacxSubKey.GetValue(KeyName)!;
            if (entropy is null)
                return null;

            //Get the saved data
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), MessengerInfos.Name);
            string loginDataPath = Path.Combine(appDataPath, LoginDataFilename);

            if (!File.Exists(loginDataPath))
            {
                return null;
            }

            //Read File
            byte[] ciphertext = File.ReadAllBytes(loginDataPath);

            //Decrypt File
            byte[] plainBytes = ProtectedData.Unprotect(ciphertext, entropy, DataProtectionScope.CurrentUser);

            //Format data
            string plaintext = Encoding.UTF8.GetString(plainBytes);
            return JsonSerializer.Deserialize<LoginData>(plaintext);
        }
        catch
        {
            return null;
        }
    }

    private static RegistryKey? GetCacxRegistry()
    {
        return Registry.CurrentUser.OpenSubKey(SoftwareSubKeyName)?.OpenSubKey(SubKeyName);
    }
}
