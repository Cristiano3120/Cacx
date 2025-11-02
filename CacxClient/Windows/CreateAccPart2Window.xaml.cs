using CacxClient.Communication.HTTPCommunication;
using CacxClient.ExtensionMethods;
using CacxClient.Helpers;
using CacxShared.ApiResources;
using CacxShared.Endpoints;
using CacxShared.SharedDTOs;
using Cristiano3120.Logging;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CacxClient.Windows;
/// <summary>
/// Interaction logic for CreateAccPart2Window.xaml
/// </summary>
public partial class CreateAccPart2Window : BaseWindow
{
    private CancellationTokenSource? _animationCts;
    private readonly Color _animatedErrorColor;
    private readonly Brush _defaultErrorBrush;
    private string? _profilePicturePath;
    private readonly User _user;

    public CreateAccPart2Window(User user)
    {
        logger.LogDebug(LoggerParams.None, $"{nameof(CreateAccPart2Window)} initialized");
        InitializeComponent();

        _animatedErrorColor = App.Current.Resources["ErrorColor"] as Color? ?? Color.FromRgb(234, 23, 31);
        _defaultErrorBrush = App.Current.Resources["DefaultErrorBrush"] as Brush ?? Brushes.LightGray;

        _user = user;

        GoBackBtn.Click += GoBackBtn_Click;
        SignUpBtn.Click += SignUpBtn_ClickAsync;

        ProfilePictureEllipse_Init();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosed(e);
        _animationCts?.Dispose();
    }

    private void ProfilePictureEllipse_Init()
    {
        ProfilePictureEllipse.MouseLeftButtonDown += ProfilePictureEllipse_Click;
        ProfilePictureEllipse.Cursor = Cursors.Hand;
    }

    private void ProfilePictureEllipse_Click(object sender, MouseButtonEventArgs args)
    {
        logger.LogDebug(LoggerParams.None, "Opening the file explorer");
        OpenFileDialog openFileDialog = new()
        {
            Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
            Title = "Select a profile picture",
            CheckFileExists = true,
            CheckPathExists = true,
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                _profilePicturePath = openFileDialog.FileName;

                ImageBrush imageBrush = new()
                {
                    ImageSource = new BitmapImage(new Uri(_profilePicturePath)),
                    Stretch = Stretch.UniformToFill
                };

                ProfilePictureEllipse.Fill = imageBrush;
            }
            catch (Exception)
            {
                const string ErrorMsg = "Failed to load the selected image file";
                logger.LogError(LoggerParams.None, ErrorMsg);
                InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, ErrorMsg, ref _animationCts);
            }
        }
    }

    private void GoBackBtn_Click(object sender, RoutedEventArgs args)
    {
        logger.LogDebug(LoggerParams.None, "Going back to the previous window");

        CreateAccWindow _createAccWindow = new();
        _createAccWindow.SetDataFromPart2(_user, false);

        GuiHelper.SwitchWindow(_createAccWindow);
    }

    private async void SignUpBtn_ClickAsync(object sender, RoutedEventArgs args)
    {
        logger.LogDebug(LoggerParams.None, "Sign up button clicked");

        User? validatedUser = await ValidateInputAsync();
        if (validatedUser is null)
        {
            return;
        }

        GuiHelper.SwitchWindow(new VerificationWindow(validatedUser, _profilePicturePath));
    }

    private async Task<User?> ValidateInputAsync()
    {
        //Both methods validate and take the needed UI steps to indicate change
        if (!LocalValidation(out string username, out string firstName, out string lastName))
        {
            return null;
        }

        if (!await ServerValidationAsync(username))
        {
            return null;
        }

        return _user with
        {
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            Biography = BiographyBox.Text,
        };
    }

    private bool LocalValidation(out string username, out string firstName, out string lastName)
    {
        username = UsernameBox.Text;
        firstName = "";
        lastName = "";

        if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
        {
            const string ErrorMsg = "Username must be at least 3 characters long";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, ErrorMsg, ref _animationCts);
            return false;
        }

        firstName = FirstNameBox.Text;
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2)
        {
            const string ErrorMsg = "Not a valid first name";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, ErrorMsg, ref _animationCts);
            return false;
        }

        lastName = LastNameBox.Text;
        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2)
        {
            const string ErrorMsg = "Not a valid last name";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, ErrorMsg, ref _animationCts);
            return false;
        }

        return true;
    }

    private async Task<bool> ServerValidationAsync(string username)
    {
        (bool requestSuccesful, bool emailFound, bool usernameFound) 
            = await CheckEmailAndUsernameAsync(_user.Email, username);

        //Something unrelated went wrong
        if (!requestSuccesful)
        {
            const string ErrorMsg = "Something went wrong :( Try again!";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, ErrorMsg, ref _animationCts);

            return false;
        }

        if (emailFound)
        {
            CreateAccWindow createAccWindow = new();
            createAccWindow.SetDataFromPart2(_user, emailInvalid: true);

            return false;
        }
        else if (usernameFound)
        {
            const string ErrorMsg = "Username already in use!";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, ErrorMsg, ref _animationCts);

            return false;
        }

        return true;
    }

    private static async Task<(bool requestSuccesful, bool emailFound, bool usernameFound)> CheckEmailAndUsernameAsync(string email, string username)
    {
        Http http = App.GetHttp();
        UniqueUserData uniqueUserData = new()
        {
            Email = email,
            Username = username
        };

        string endpoint = Endpoints.GetAuthEndpoint(AuthEndpoint.CheckUserUniqueness);
        ApiResponse<(bool emailFound, bool usernameFound)> response =
            await http.PostAsync<UniqueUserData, (bool emailFound, bool usernameFound)>(uniqueUserData, endpoint, CallerInfos.Create());
    

        return (response.IsSuccess, response.Data.emailFound, response.Data.usernameFound);
    }

    public void SetUserData(User user)
    {
        UsernameBox.Text = user.Username;
        FirstNameBox.Text = user.FirstName;
        LastNameBox.Text = user.LastName;
        BiographyBox.Text = user.Biography;
    }
}