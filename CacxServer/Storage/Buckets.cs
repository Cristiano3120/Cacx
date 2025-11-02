namespace CacxServer.Storage;

public static class Buckets
{
    public const string Users = "user";

    public static readonly IReadOnlyList<string> All =
    [
        Users,
    ];
}
