using CacxClient.Communication.HTTPCommunication;
using CacxClient.Helpers;
using CacxShared.Endpoints;
using CacxShared.SharedDTOs;
using Cristiano3120.Logging;

namespace CacxClient.Windows;
/// <summary>
/// Interaction logic for VerificationWindow.xaml
/// </summary>
public partial class VerificationWindow : BaseWindow
{
    private readonly User _user;
    public VerificationWindow(User user, byte[] profilePictureBytes)
    {
        InitializeComponent();

        _ = SendVerificationEmailAsync();

        GoBackBtn.Click += GoBackBtn_Click;
        VerifyBtn.Click += VerifyBtn_Click;

        _user = user;
    }

    private void VerifyBtn_Click(object sender, System.Windows.RoutedEventArgs e)
    {

    }

    private void GoBackBtn_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        CreateAccPart2Window createAccPart2Window = new(_user);
        createAccPart2Window.SetUserData(_user);

        GuiHelper.SwitchWindow(createAccPart2Window);
    }

    private async Task SendVerificationEmailAsync()
    {
        Http http = App.GetHttp();
        string endpoint = Endpoints.GetAuthEndpoint(AuthEndpoint.TwoFactorAuth);
        UniqueUserData uniqueUserData = new()
        {
            Email = _user.Email,
            Username = _user.Username,
        };

        _ = await http.PostAsync<UniqueUserData, object>(uniqueUserData, endpoint, CallerInfos.Create());
    }
}
