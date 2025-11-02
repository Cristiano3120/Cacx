using System.Text.Json;

namespace CacxShared.SharedDTOs;

public sealed record ProfilePictureUploadRequest : IMultipartFormData
{
    public ulong UserId { get; init; }
    public string FileName { get; init; } = null!;
    public Stream FileStream { get; init; } = null!;

    public MultipartFormDataContent ToMultipartContent()
    {
        const string FileFieldName = "file";

        MultipartFormDataContent content = new()
        {
            { new StringContent(UserId.ToString()), nameof(UserId) },
            { new StreamContent(FileStream), FileFieldName, FileName }
        };

        Console.WriteLine(JsonSerializer.Serialize(content, new JsonSerializerOptions() { WriteIndented = true}));

        return content;
    }
}
