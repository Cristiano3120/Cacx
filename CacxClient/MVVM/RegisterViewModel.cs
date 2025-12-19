using CacxClient.Abstractions;
using CacxClient.Commands;
using CacxClient.RandomPasswordGenerator;
using CacxClient.Services;
using CacxClient.Services.RateLimiter;
using Cristiano3120.Logging;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CacxClient.MVVM;

public class RegisterViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<string, Color>? OnDisplayInformation;
    public ICommand RegisterCommand { get; }
    public ICommand GeneratePasswordCommand { get; }

    private readonly ICursorService _cursorService;
    private readonly IAuthService _authService;
    private readonly IRateLimiter _rateLimiter;
    private readonly Logger _logger;

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

    public RegisterViewModel(IAuthService authService, ICursorService cursorService, Logger logger)
    {
        logger.LogInformation(LoggerParams.None, () => "RegisterViewModel initialized");

        RegisterCommand = new RelayCommand(async (_) => await RegisterAsync(), CanRegister); 
        GeneratePasswordCommand = new RelayCommand(async (_) =>
        {
            Password = new PasswordGenerator().GeneratePassword(20);
            Clipboard.SetText(Password);

            Color? color = ThemeManager.GetColor(key: "TextPrimaryColor");
            color ??= Colors.LightGray;

            OnDisplayInformation?.Invoke("Copied to clipboard", color.Value);
        });   

        _rateLimiter = RateLimiters.Register;
        _cursorService = cursorService;
        _authService = authService;
        _logger = logger;

        Email = string.Empty;
        Username = string.Empty;
        DisplayName = string.Empty;
        Password = string.Empty;
    }

    public async Task RegisterAsync()
    {
        
    }

    public bool CanRegister(object? sender)
    {
        return true;
    }

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
