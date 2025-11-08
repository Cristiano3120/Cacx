using CacxClient.Communication.HTTPCommunication;
using CacxClient.ExtensionMethods;
using CacxClient.Helpers;
using CacxClient.PasswordGeneratorResources;
using CacxShared.ApiResources;
using CacxShared.Endpoints;
using CacxShared.SharedDTOs;
using Cristiano3120.Logging;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace CacxClient.Windows;

/// <summary>
/// Interaction logic for CreateAccWindow.xaml
/// </summary>
public partial class CreateAccWindow : BaseWindow
{
    private CancellationTokenSource? _animationCts;
    private readonly Color _animatedErrorColor;
    private readonly Brush _defaultErrorBrush;

    public CreateAccWindow()
    {
        logger.LogDebug(LoggerParams.None, $"{nameof(CreateAccWindow)} initialized.");

        InitializeComponent();
        InitBirthdayBoxes();

        GeneratePwBtn.Click += GeneratePwBtn_Click;
        ContinueBtn.Click += ContinueBtn_ClickAsync;
        GoBackBtn.Click += GoBackBtn_ClickAsync;

        _animatedErrorColor = App.Current.Resources["ErrorColor"] as Color? ?? Color.FromRgb(234, 23, 31);
        _defaultErrorBrush = App.Current.Resources["DefaultErrorBrush"] as Brush ?? Brushes.LightGray;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _animationCts?.Dispose();
        base.OnClosing(e);
    }

    /// <summary>
    /// Populates the form fields with data from the specified user object.
    /// </summary>
    /// <remarks>This method updates the text boxes and selection controls to reflect the values from the
    /// provided user. <c> This Method is used when the user goes back from the CreateAccPart2 Window to this.</c></remarks>
    /// <param name="user">The user whose email, password, and birthday information will be used to set the corresponding form fields.
    /// Cannot be null.</param>
    public void SetDataFromPart2(User user, bool emailInvalid)
    {
        EmailTextBox.Text = user.Email;
        PasswordTextBox.Text = user.Password;
        DayBox.SelectedItem = user.Birthday.Day;
        MonthBox.SelectedItem = user.Birthday.Month;
        YearBox.SelectedItem = user.Birthday.Year;

        if (emailInvalid)
        {
            const string ErrorMsg = "Email already in use!";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, ErrorMsg, ref _animationCts);
        }
    }

    private async void GoBackBtn_ClickAsync(object sender, RoutedEventArgs args)
    {
        GuiHelper.SwitchWindow<LoginWindow>();
    }

    private async void ContinueBtn_ClickAsync(object sender, RoutedEventArgs e)
    {
        User? user = await ValidateInputAsync();
        if (user is null)
        {
            return;
        }

        GuiHelper.SwitchWindow(new CreateAccPart2Window(user));
    }

    private async Task<User?> ValidateInputAsync()
    {
        string email = EmailTextBox.Text;
        string password = PasswordTextBox.Text;
        (bool succesful, DateOnly birthday) = await LocalValidationAsync(email, password);

        if (!succesful)
        {
            return null;
        }

        (bool requestSuccesful, bool emailFound) = await CheckEmailAsync(email);
        if (!requestSuccesful)
        {
            const string ErrorMsg = "Something went wrong :( Try again!";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, ErrorMsg, ref _animationCts);

            return null;
        }

        if (emailFound)
        {
            const string ErrorMsg = "Email already in use!";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, ErrorMsg, ref _animationCts);

            return null;
        }

        return new()
        {
            Email = email,
            Password = password,
            Birthday = birthday
        };
    }

    private async Task<(bool, DateOnly)> LocalValidationAsync(string email, string password)
    {
        if (!await Helper.IsEmailValidAsync(email))
        {
            const string ErrorMsg = "Invalid email";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, ErrorMsg, ref _animationCts);
            return (false, default);
        }

        const byte MinPasswordLength = 8;
        if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
        {
            string ErrorMsg = $"Password length has to be greater than {MinPasswordLength - 1}";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, ErrorMsg, ref _animationCts);
            return (false, default);
        }

        if (DayBox.SelectedItem is not int day ||
             MonthBox.SelectedItem is not int month ||
             YearBox.SelectedItem is not int year ||
            !DateOnly.TryParse($"{year}-{month}-{day}", CultureInfo.InvariantCulture, out DateOnly birthday))
        {
            const string ErrorMsg = "Invalid birthday";
            InfoTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, ErrorMsg, ref _animationCts);
            return (false, default);
        }

        return (true, birthday);
    }

    private static async Task<(bool requestSuccesful, bool emailFound)> CheckEmailAsync(string email)
    {
        Http http = App.GetHttp();
        UniqueUserData uniqueUserData = new()
        {
            Email = email,
            Username = ""
        };

        string endpoint = Endpoints.GetAuthEndpoint(AuthEndpoint.CheckUserUniqueness);
        ApiResponse<(bool emailFound, bool usernameFound)> response =
            await http.PostAsync<UniqueUserData, (bool emailFound, bool usernameFound)>(uniqueUserData, endpoint, CallerInfos.Create());

        return (response.IsSuccess, response.Data.emailFound);
    }

    private void GeneratePwBtn_Click(object sender, RoutedEventArgs e)
    {
        string password = PasswordTextBox.Text = PasswordGenerator.GeneratePassword();
        Clipboard.SetText(password);

        SolidColorBrush startingBrush = new(Color.FromRgb(204, 204, 204));
        Color animatedColor = Color.FromRgb(102, 102, 102);

        const string Msg = "Copied to the clipboard!";
        InfoTextBlock.TriggerDisplayAnimation(startingBrush, animatedColor, Msg,  ref _animationCts);

        PasswordTextBox.Text = password;
    }

    private void InitBirthdayBoxes()
    {
        for (int i = 1; i <= 31; i++)
        {
            _ = DayBox.Items.Add(i);
        }

        for (int i = 1; i <= 12; i++)
        {
            _ = MonthBox.Items.Add(i);
        }

        int currentYear = DateTime.Now.Year;
        for (int i = currentYear; i >= 1930; i--)
        {
            _ = YearBox.Items.Add(i);
        }
    }
}
