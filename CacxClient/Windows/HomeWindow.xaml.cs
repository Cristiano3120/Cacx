using CacxClient.Communication.HTTPCommunication;
using CacxShared.Endpoints;
using CacxShared.SharedDTOs;
using CacxShared.SharedMinioResources;
using Cristiano3120.Logging;
using System.IO;
using System.Windows.Controls;

namespace CacxClient.Windows;

/// <summary>
/// Interaction logic for HomeWindow.xaml
/// </summary>
public partial class HomeWindow : UserControl
{
    private Logger _logger;
    private Http _http;
    private User _user;

    public HomeWindow(User user, FileStream? profilePictureStream)
    {
        InitializeComponent();

        _logger = App.GetLogger();
        _http = App.GetHttp();
        _user = user;

        if (profilePictureStream is null)
        {
            byte[] bytes = _http.GetBytesAsync(Endpoints.GetCdnEndpoint(Bucket.User, user.ProfilePictureUrl), callerInfos: CallerInfos.Create()).Result;
            Console.WriteLine(bytes.Length);
        }
    }
}
