using System.Text.Json;

namespace CacxClient;

internal static class Helper
{
    public static JsonElement GetConfig()
    {
        const string Filepath = "appSettings.json";
        return JsonDocument.Parse(Filepath).RootElement;
    }
}
