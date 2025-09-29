using CacxClient.Communication.HTTPCommunication;
using Cristiano3120.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CacxClient.Windows;

/// <summary>
/// Interaction logic for Login.xaml
/// </summary>
public partial class LoginWindow : BaseWindow
{
    private readonly Logger _logger;
    private readonly Http _http;
    public LoginWindow()
    {
        InitializeComponent();
        LoginBtn.Click += LoginBtn_Click;

        _logger = App.ServiceProvider.GetRequiredService<Logger>();
        _http = App.ServiceProvider.GetRequiredService<Http>();

        _logger.LogDebug(LoggerParams.None, "LoginWindow initialized");
    }

    public void LoginBtn_Click(object sender, RoutedEventArgs args)
    {
        
    }
}
