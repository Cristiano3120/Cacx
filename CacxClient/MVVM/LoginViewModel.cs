using CacxClient.Commands;
using CacxClient.Helper;
using CacxClient.Interfaces;
using Cristiano3120.Logging;
using System.ComponentModel;
using System.Windows.Input;

namespace CacxClient.MVVM;

public class LoginViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<string>? OnInvalidData;
    private readonly IAuthService _authService;
    private readonly Logger _logger;

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

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public LoginViewModel(IAuthService authService, Logger logger)
    {
        LoginCommand = new RelayCommand(async (_) => await LoginAsync(), CanLogin); //TODO: RateLimiter

        _authService = authService;
        _logger = logger;

        Email = string.Empty;
        Password = string.Empty;
    }

    private async Task LoginAsync()
    {
        _logger.LogInformation(LoggerParams.None, () => "Attempting to log in");

        if (!await NetworkHelper.IsEmailValidAsync(Email))
        {
            const string ErrorMsg = "The entered Email is invalid";
            OnInvalidData?.Invoke(ErrorMsg);

            return;
        }

        await _authService.LoginAsync();
    }

    private bool CanLogin(object? _)
    {
        _logger.LogInformation(LoggerParams.None, () => "Checking if login is possible");

        if (string.IsNullOrEmpty(Email))
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
