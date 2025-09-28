using CacxShared.Helper;
using System.IO;
using System.Text.Json;

namespace CacxClient.Helpers;

internal static class Helper
{
    public static JsonElement GetConfig()
    {
        string Filepath = SharedHelper.GetDynamicPath("appSettings.json");
        return JsonDocument.Parse(File.ReadAllText(Filepath)).RootElement;
    }
}
