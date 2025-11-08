using CacxClient.Communication.HTTPCommunication;
using CacxClient.ExtensionMethods;
using CacxClient.Helpers;
using CacxShared.ApiResources;
using CacxShared.Endpoints;
using CacxShared.SharedDTOs;
using Cristiano3120.Logging;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace CacxClient.Windows;
/// <summary>
/// Interaction logic for VerificationWindow.xaml
/// </summary>
public partial class VerificationWindow : BaseWindow
{
    private CancellationTokenSource? _animationCts;
    private readonly string _profilePicturePath;
    private readonly Color _animatedErrorColor;
    private readonly Brush _defaultErrorBrush;
    private readonly User _user;

    public VerificationWindow(User user, string profilePicturePath)
    {
        InitializeComponent();

        _user = user;
        _animatedErrorColor = App.Current.Resources["ErrorColor"] as Color? ?? Color.FromRgb(234, 23, 31);
        _defaultErrorBrush = App.Current.Resources["DefaultErrorBrush"] as Brush ?? Brushes.LightGray;

        _ = RequestVerificationEmailAsync(user);

        GoBackBtn.Click += GoBackBtn_Click;
        VerifyBtn.Click += VerifyBtn_ClickAsync;
        _profilePicturePath = profilePicturePath;
    }

    private async void VerifyBtn_ClickAsync(object sender, RoutedEventArgs e)
    {
        string verificationCodeStr = CodeTextBox.Text;
        string errorMsg;

        if (string.IsNullOrEmpty(verificationCodeStr) || !int.TryParse(verificationCodeStr, out int verificationCode))
        {
            errorMsg = "The code you entered is invalid!";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, errorMsg, ref _animationCts);

            return;
        }

        ApiResponse<bool> response =  await VerifyAsync(verificationCode);
        if (!response.IsSuccess)
        {
            errorMsg = "Something went wrong :( Try Again!";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, errorMsg, ref _animationCts);

            return;
        }

        if (!response.Data)
        {
            errorMsg = "The code you entered is either wrong or invalid!";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, errorMsg, ref _animationCts);

            return;
        }

        User? createdUser = await CreateUserAsync();
        if (createdUser is null)
        {
            errorMsg = "Something went wrong :( Try Again!";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, errorMsg, ref _animationCts);
            
            return;
        }

        FileStream? profilePictureStream = default;
        if (!string.IsNullOrEmpty(_profilePicturePath))
        {
            (string url, profilePictureStream) = await UploadProfilePictureAsync(createdUser);
            createdUser = createdUser with
            {
                ProfilePictureUrl = url,
            };
        }

        HomeWindow homeWindow = new(createdUser, profilePictureStream);
        GuiHelper.SwitchWindow(homeWindow);
    }

    private async Task<ApiResponse<bool>> VerifyAsync(int verificationCode)
    {
        string verifyEndpoint = Endpoints.GetAuthEndpoint(AuthEndpoint.Verify);
        CallerInfos callerInfos = CallerInfos.Create();

        return await http.PostAsync<VerificationRequestData, bool>(new(_user.Username, verificationCode), verifyEndpoint, callerInfos);
    }

    private async Task<(string url, FileStream fileStream)> UploadProfilePictureAsync(User createdUser)
    {
        string uploadEndpoint = Endpoints.GetAuthEndpoint(AuthEndpoint.UploadProfilePicture);
        int indexOfLastPathSeperator = _profilePicturePath.LastIndexOf(Path.PathSeparator);

        FileStream fileStream = File.OpenRead(_profilePicturePath);
        ProfilePictureUploadRequest request = new()
        {
            FileStream = fileStream,
            FileName = $"{_profilePicturePath[++indexOfLastPathSeperator..]}",
            UserId = createdUser.Id,
        };

        _ = http.PostAsync<ProfilePictureUploadRequest, object>(request, uploadEndpoint, CallerInfos.Create());
        return ($"{createdUser.Id}/profilePicture{GetFileType(_profilePicturePath)}", fileStream);
    }

    private static string GetFileType(string filePath)
    {
        int indexOfLastDot = filePath.LastIndexOf('.');
        return filePath[indexOfLastDot..];
    }

    private async Task<User?> CreateUserAsync()
    {
        CallerInfos callerInfos = CallerInfos.Create();
        string endpoint = Endpoints.GetAuthEndpoint(AuthEndpoint.CreateAcc);

        ApiResponse<User> apiResponse = await http.PostAsync<User, User>(_user, endpoint, callerInfos);
        return apiResponse.Data;
    }

    private void GoBackBtn_Click(object sender, RoutedEventArgs e)
    {
        CreateAccPart2Window createAccPart2Window = new(_user);
        createAccPart2Window.SetUserData(_user);

        GuiHelper.SwitchWindow(createAccPart2Window);
    }

    private async Task RequestVerificationEmailAsync(User user)
    {
        string endpoint = Endpoints.GetAuthEndpoint(AuthEndpoint.StartTwoFactorAuth);
        UniqueUserData uniqueUserData = new()
        {
            Email = user.Email,
            Username = user.Username,
        };

        _ = await http.PostAsync<UniqueUserData, object>(uniqueUserData, endpoint, CallerInfos.Create());
    }
}
