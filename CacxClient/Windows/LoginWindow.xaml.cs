using CacxClient.Communication.HTTPCommunication;
using CacxClient.Helpers;
using Cristiano3120.Logging;
using System.Windows;

namespace CacxClient.Windows;

/// <summary>
/// Interaction logic for Login.xaml
/// </summary>
public partial class LoginWindow : BaseWindow
{
    private readonly Http _http;
    public LoginWindow()
    {
        logger.LogDebug(LoggerParams.None, $"{nameof(LoginWindow)} initialized");
        _http = App.GetHttp();

        InitializeComponent();
        LoginBtn.Click += LoginBtn_Click;

        GuiHelper.SwitchWindow<CreateAccWindow>();
    }

    public void LoginBtn_Click(object sender, RoutedEventArgs args)
    {
        throw new NotImplementedException($"NOT IMPLEMENTED {nameof(LoginWindow)} L.28");
    }
}
