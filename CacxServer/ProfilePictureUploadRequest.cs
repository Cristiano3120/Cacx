namespace CacxServer;

public class ProfilePictureUploadRequestServerSide
{
    public ulong UserId { get; set; }
    public IFormFile File { get; set; } = null!;
}
