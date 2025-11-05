namespace CacxShared.SharedMinioResources;

/// <summary>
/// Call <see cref="BucketExtensions.ToBucketName(Bucket)"/> to get the bucket name.
/// </summary>
public enum Bucket
{
    User
}

public static class BucketExtensions
{
    /// <summary>
    /// Converts the specified <see cref="Bucket"/> value to its corresponding bucket name in lowercase format.
    /// </summary>
    /// <param name="bucket">The <see cref="Bucket"/> value to convert to a bucket name.</param>
    /// <returns>A string containing the lowercase name of the specified bucket.</returns>
    public static string ToBucketName(this Bucket bucket)
        => $"{bucket}".ToLower();

    /// <summary>
    /// Gets the name of all the Buckets.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> GetAllBucketNames()
    {
        foreach (string bucketName in Enum.GetNames<Bucket>())
        {
            yield return bucketName.ToLower();
        }
    }
}
