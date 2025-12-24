using CacxClient.Abstractions;
using CacxClient.Commands;
using CacxClient.Helper;
using CacxClient.RandomPasswordGenerator;
using CacxClient.Services;
using CacxClient.Services.RateLimiter;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CacxShared.SharedDTOs;
using CacxShared.Abstractions;
using CacxClient.Abstractions.Auth;

namespace CacxClient.MVVM;

public class RegisterViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<string, Color>? OnDisplayInformation;
    public event Action<bool>? OnRequestRunningStateChanged;
    public ICommand RegisterCommand { get; }
    public ICommand GeneratePasswordCommand { get; }

    private readonly IAuthService _authService;
    private readonly IRateLimiter _rateLimiter;
    private bool _isRequestRunning;

    public string Email
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    public string Username
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    public string DisplayName
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(DisplayName));
        }
    }

    public string Password
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    public bool RegisterBtnEnabled
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(RegisterBtnEnabled));
        }
    }

    public bool LoginBtnEnabled
    {
        get => field;
        set 
        {
            field = value;
            OnPropertyChanged(nameof(LoginBtnEnabled));
        }
    }


    public RegisterViewModel(IAuthService authService, ICursorService cursorService)
    {
        RegisterCommand = new RelayCommand(async (_) => await RegisterAsync(), CanRegister); 
        GeneratePasswordCommand = new RelayCommand(async (_) =>
        {
            Password = new PasswordGenerator().GeneratePassword(20);
            Clipboard.SetText(Password);

            Color? color = ThemeManager.GetColor(key: "TextPrimaryColor");
            color ??= Colors.LightGray;

            OnDisplayInformation?.Invoke("Copied to clipboard", color.Value);
        });

        OnRequestRunningStateChanged += isRequestRunning =>
        {
            LoginBtnEnabled = !isRequestRunning;
            _isRequestRunning = isRequestRunning;
            CommandManager.InvalidateRequerySuggested();

            if (isRequestRunning)
            {
                cursorService.SetCursor(Cursors.Wait);
            }
            else
            {
                cursorService.ResetCursor();
            }
        };

        _rateLimiter = RateLimiters.Register;
        _authService = authService;

        Email = string.Empty;
        Username = string.Empty;
        DisplayName = string.Empty;
        Password = string.Empty;
        RegisterBtnEnabled = true;
    }

    private async Task RegisterAsync()
    {
        const string ErrorColorKey = "TextErrorColor";
        if (!_rateLimiter.TryConsume())
        {
            const string ErrorMsg = "Don´t spam :( You gotta wait a bit!";
            OnDisplayInformation?.Invoke(ErrorMsg, ThemeManager.GetColor(key: ErrorColorKey, Colors.Red));
            return;
        }

        if (! await ValidateDataAsync())
            return;
        
        OnRequestRunningStateChanged?.Invoke(true);
        RegisterResult result = await _authService.RegisterAsync(new RegisterRequest()
        {
            Email = Email,
            Username = Username,
        });
        OnRequestRunningStateChanged?.Invoke(false);


        if (!result.IsSuccess)
            OnDisplayInformation?.Invoke(result.ErrorMessage!, ThemeManager.GetColor(key: ErrorColorKey, Colors.Red));

        //TODO: Use the RegisterResult in the MVVM | ?Save Token | Switch screen
        //TODO: Program.cs and App.xaml.cs clean up
        //TODO: Auth request limiter for server | ?Ip-based
        //TODO: Server soll http request responses loggen und falsche paths mit nem entsprechenden http code beantworten
    }

    private async Task<bool> ValidateDataAsync()
    {
        Color errorColor = ThemeManager.GetColor(key: "TextErrorColor", Colors.Red);

        if (string.IsNullOrEmpty(Email) || !await NetworkHelper.IsEmailValidAsync(Email))
        {
            const string ErrorMsg = "The entered email is invalid";
            OnDisplayInformation?.Invoke(ErrorMsg, errorColor);

            return false;
        }

        const byte MinPasswordLength = 8;
        if (string.IsNullOrEmpty(Email) || Password.Length < MinPasswordLength)
        {
            const string ErrorMsg = "Password must be at least 8 characters long";
            OnDisplayInformation?.Invoke(ErrorMsg, errorColor);
            return false;
        }

        if (string.IsNullOrEmpty(Username) || Username.Any(x => !char.IsLetterOrDigit(x) && x != '_' && x != '-'))
        {
            const string ErrorMsg = "Username can only contain letters, digits, underscores and hyphens";
            OnDisplayInformation?.Invoke(ErrorMsg, errorColor);
            return false;
        }

        if (string.IsNullOrEmpty(DisplayName))
        {
            const string ErrorMsg = "Display name cannot be empty";
            OnDisplayInformation?.Invoke(ErrorMsg, errorColor);
            return false;
        }

        return true;
    }

    public bool CanRegister(object? sender)
        => !_isRequestRunning;

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
