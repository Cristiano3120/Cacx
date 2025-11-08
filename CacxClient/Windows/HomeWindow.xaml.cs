using CacxShared.Endpoints;
using CacxShared.SharedDTOs;
using CacxShared.SharedMinioResources;
using Cristiano3120.Logging;
using System.IO;

namespace CacxClient.Windows;

/// <summary>
/// Interaction logic for HomeWindow.xaml
/// </summary>
public partial class HomeWindow : BaseWindow
{
    private User _user;

    public HomeWindow(User user, FileStream? profilePictureStream)
    {
        InitializeComponent();
        _user = user;

        if (profilePictureStream is null)
        {
            byte[] bytes = http.GetBytesAsync(Endpoints.GetCdnEndpoint(Bucket.User, user.ProfilePictureUrl), callerInfos: CallerInfos.Create()).Result;
            Console.WriteLine(bytes.Length);
        }
    }
}
