namespace CacxServer.Storage;

public static class ObjectPaths
{
    public static string GetUserProfilePicturePath(ulong userId)
        => $"{userId}/ProfilePicture";
}
