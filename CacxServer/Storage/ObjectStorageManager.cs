using CacxShared.SharedDTOs;
using Cristiano3120.Logging;
using Minio;
using Minio.DataModel.Args;

namespace CacxServer.Storage;

public sealed class ObjectStorageManager
{
    private readonly IMinioClient _minioClient;
    private readonly Logger _logger;

    public ObjectStorageManager(IMinioClient minioClient, Logger logger)
    {
        _minioClient = minioClient;
        _logger = logger;

        _ = InitBucketsAsync();
    }

    public async Task InitBucketsAsync()
    {
        try
        {
            foreach (string bucket in Buckets.All)
            {
                if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket)))
                {
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerParams.None, ex, CallerInfos.Create());
        }
    }

    public async Task<bool> UploadProfilePictureAsync(ProfilePictureUploadRequestServerSide uploadRequest)
    {
        try
        {
            _logger.LogInformation(LoggerParams.None, $"Uploading profile picture for user {uploadRequest.UserId}", CallerInfos.Create());

            string objectName = ObjectPaths.GetUserProfilePicturePath(uploadRequest.UserId);
            await using Stream stream = uploadRequest.File.OpenReadStream();

            PutObjectArgs putObjectArgs = new PutObjectArgs()
                .WithBucket(Buckets.Users)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(uploadRequest.File.ContentType);

            _ = await _minioClient.PutObjectAsync(putObjectArgs);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerParams.None, ex, CallerInfos.Create());
            return false;
        }
    }
}
