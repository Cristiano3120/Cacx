using CacxShared.Helper;
using DnsClient;
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

    public static async Task<bool> DomainHasMxRecordAsync(string email)
    {
        try
        {
            string domain = email.Split('@')[1];
            LookupClient lookup = new();
            IDnsQueryResponse result = await lookup.QueryAsync(domain, QueryType.MX);

            return result.Answers.MxRecords().Any();
        }
        catch
        {
            return false;
        }
    }
}
