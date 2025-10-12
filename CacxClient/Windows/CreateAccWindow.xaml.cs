using CacxClient.ExtensionMethods;
using CacxClient.Helpers;
using CacxClient.PasswordGeneratorResources;
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
        InitializeComponent();
        InitBirthdayBoxes();

        GeneratePwBtn.Click += GeneratePwBtn_ClickAsync;
        ContinueBtn.Click += ContinueBtn_ClickAsync;
        GoBackBtn.Click += GoBackBtn_Click;

        _animatedErrorColor = App.Current.Resources["ErrorColor"] as Color? ?? Color.FromRgb(234, 23, 31);
        _defaultErrorBrush = App.Current.Resources["DefaultErrorBrush"] as Brush ?? Brushes.LightGray;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _animationCts?.Dispose();
        base.OnClosing(e);
    }

    private void GoBackBtn_Click(object sender, RoutedEventArgs args)
        => GuiHelper.SwitchWindow<LoginWindow>();

    private async void ContinueBtn_ClickAsync(object sender, RoutedEventArgs e)
    {
        string email = EmailTextBox.Text;
        if (!await Helper.IsEmailValidAsync(email))
        {
            ErrorTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, "Invalid email",  ref _animationCts);
            return;
        }

        string password = PasswordTextBox.Text;
        if (string.IsNullOrEmpty(password) || password.Length < 8)
        {
            ErrorTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, "Password length has to be greater than 7",  ref _animationCts);
            return;
        }

        if (DayBox.SelectedItem is not int day ||
             MonthBox.SelectedItem is not int month ||
             YearBox.SelectedItem is not int year ||
            !DateOnly.TryParse($"{year}-{month}-{day}", CultureInfo.InvariantCulture, out DateOnly birthday))
        {
            ErrorTextBlock.TriggerDisplayAnimation(_defaultErrorBrush, _animatedErrorColor, "Invalid birthday",  ref _animationCts);
            return;
        }

        GuiHelper.SwitchWindow(new CreateAccPart2Window(email, password, birthday));
    }

    private async void GeneratePwBtn_ClickAsync(object sender, RoutedEventArgs e)
    {
        string password = PasswordTextBox.Text = PasswordGenerator.GeneratePassword();
        Clipboard.SetText(password);

        SolidColorBrush startingBrush = new(Color.FromRgb(204, 204, 204));
        Color animatedColor = Color.FromRgb(102, 102, 102);

        ErrorTextBlock.TriggerDisplayAnimation(startingBrush, animatedColor, "Copied to the clipboard!",  ref _animationCts);

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
