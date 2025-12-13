using CacxClient.Commands;
using CacxClient.Helper;
using CacxClient.Abstractions;
using CacxClient.Services.RateLimiter;
using CacxShared.SharedDTOs;
using Cristiano3120.Logging;
using System.ComponentModel;
using System.Windows.Input;

namespace CacxClient.MVVM;

public class LoginViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<bool>? OnRequestRunningStateChanged;
    public event Action<string>? OnInvalidData;

    private readonly ICursorService _cursorService;
    private readonly IAuthService _authService;
    private readonly IRateLimiter _rateLimiter;

    private readonly Logger _logger;
    private bool _isRequestRunning;

    public ICommand LoginCommand { get; }

    public string Email 
    { 
        get => field; 
        set
        { 
            field = value;
            OnPropertyChanged(nameof(Email));
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

    public bool LoginBtnEnabled
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(LoginBtnEnabled));
        }
    }

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public LoginViewModel(IAuthService authService, ICursorService cursorService, Logger logger)
    {
        LoginCommand = new RelayCommand(async (_) => await LoginAsync(), CanLogin);
        LoginBtnEnabled = true;
        
        Email = string.Empty;
        Password = string.Empty;

        _rateLimiter = RateLimiters.Login;
        _cursorService = cursorService;
        _authService = authService;

        _isRequestRunning = false;
        _logger = logger;

        OnRequestRunningStateChanged += isRequestRunning =>
        {
            LoginBtnEnabled = !isRequestRunning;
            _isRequestRunning = isRequestRunning;
            CommandManager.InvalidateRequerySuggested();

            if (isRequestRunning)
            {
                _cursorService.SetCursor(Cursors.Wait);
            }
            else
            {
                _cursorService.ResetCursor();
            }
        };
    }

    private async Task LoginAsync()
    {
        if (!_rateLimiter.TryConsume())
        {
            const string ErrorMsg = "Don´t spam :( You gotta wait a bit!";
            OnInvalidData?.Invoke(ErrorMsg);
            return;
        }

        if (!await ValidateDataAsync())
        {
            return;
        }

        OnRequestRunningStateChanged?.Invoke(true);

        _logger.LogInformation(LoggerParams.None, () => "Attempting to log in");
        await _authService.LoginAsync(new LoginRequest() 
        {
            Email = Email, 
            Password = Password 
        });

        OnRequestRunningStateChanged?.Invoke(false);
    }

    private bool CanLogin(object? _)
        => !_isRequestRunning;

    private async Task<bool> ValidateDataAsync()
    {
        Console.WriteLine(Password);
        _logger.LogInformation(LoggerParams.None, () => "Checking if login is possible");

        if (string.IsNullOrEmpty(Email) || !await NetworkHelper.IsEmailValidAsync(Email))
        {
            const string ErrorMsg = "You have to enter a Email";
            OnInvalidData?.Invoke(ErrorMsg);

            return false;
        }

        const byte MinPasswordLength = 8;
        if (string.IsNullOrEmpty(Password) || Password.Length < MinPasswordLength)
        {
            const string ErrorMsg = "Password must be at least 8 characters long";
            OnInvalidData?.Invoke(ErrorMsg);

            return false;
        }

        return true;
    }
}
