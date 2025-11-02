namespace CacxShared;

/// <summary>
/// Implement this Interface if a class you wanna send via HTTP contains a stream
/// </summary>
public interface IMultipartFormData
{
    public MultipartFormDataContent ToMultipartContent();
}
