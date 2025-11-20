using DnsClient;

namespace CacxClient.Helper;

internal static class NetworkHelper
{
    public static async Task<bool> IsEmailValidAsync(string email)
    {
#if !DEBUG
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            MailAddress addr = new(email);
            return addr.Address == email && await DomainHasMxRecordAsync(email);
        }
        catch
        {
            return false;
        }
#else
        // In debug mode, skip email validation to facilitate testing with placeholder emails.
        await Task.Delay(0); // Placeholder to keep the method async
        return true;
#endif
    }

    private static async Task<bool> DomainHasMxRecordAsync(string email)
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
