using CacxClient.Communication.HTTPCommunication;
using CacxClient.ExtensionMethods;
using CacxClient.Helpers;
using CacxShared.ApiResources;
using Cristiano3120.Logging;
using CacxShared.SharedDTOs;
using System.Windows;
using System.Windows.Media;
using CacxShared.Endpoints;

namespace CacxClient.Windows;

/// <summary>
/// Interaction logic for Login.xaml
/// </summary>
public partial class LoginWindow : BaseWindow
{
    private CancellationTokenSource? _animationCts;
    private readonly Color _animatedErrorColor;
    private readonly Brush _defaultErrorBrush;

    public LoginWindow()
    {
        _animatedErrorColor = App.Current.Resources["ErrorColor"] as Color? ?? Color.FromRgb(234, 23, 31);
        _defaultErrorBrush = App.Current.Resources["DefaultErrorBrush"] as Brush ?? Brushes.LightGray;
        logger.LogDebug(LoggerParams.None, $"{nameof(LoginWindow)} initialized");

        InitializeComponent();

        LoginBtn.Click += LoginBtn_ClickAsync;
        CreateAccHyperlink.Click += AccCreationHyperlink_Click;
    }

    public void AccCreationHyperlink_Click(object sender, RoutedEventArgs args)
        => GuiHelper.SwitchWindow(new CreateAccWindow());
    
    public async void LoginBtn_ClickAsync(object sender, RoutedEventArgs args)
    {
        if (!await ValidateInputAsync())
        {
            return;
        }

        LoginRequest loginRequest = new()
        {
            Email = EmailTextBox.Text,
            Password = PasswordTextBox.Text
        };

        string endpoint = Endpoints.GetAuthEndpoint(AuthEndpoint.Login);
        CallerInfos callerInfos = CallerInfos.Create();
        ApiResponse<User> response = await http.PostAsync<LoginRequest, User>(loginRequest, endpoint, callerInfos);
        
        if (response.Data is null)
        {
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, $"Wrong login data!", ref _animationCts);
            return;
        }

        HomeWindow homeWindow = new(response.Data!, null);
        GuiHelper.SwitchWindow(homeWindow);
    }

    /// <summary>
    /// Checks whether the input provided by the user is valid and displays an error message if not.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ValidateInputAsync()
    {
        string email = EmailTextBox.Text;
        string password = PasswordTextBox.Text;
        string errorMsg;
        
        if (string.IsNullOrEmpty(email) || !await Helper.IsEmailValidAsync(email))
        {
            errorMsg = "The email you entered is invalid!";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, errorMsg, ref _animationCts);
            return false;
        }

        if (string.IsNullOrEmpty(password) || password.Length < 8)
        {
            errorMsg = "The password you entered is invalid!";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, errorMsg, ref _animationCts);
            return false;
        }

        return true;
    }
}
