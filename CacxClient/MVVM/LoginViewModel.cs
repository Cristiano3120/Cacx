using Cacx.LocalizationManager.Abstractions;
using Cacx.LocalizationManager.Core;
using CacxClient.Abstractions;
using CacxClient.Abstractions.Auth;
using CacxClient.Commands;
using CacxClient.Extensions;
using CacxClient.Helper;
using CacxClient.Resources;
using CacxClient.Services.RateLimiter;
using CacxClient.Windows;
using CacxShared.Abstractions;
using Cristiano3120.Logging;
using System.ComponentModel;
using System.Windows.Input;

namespace CacxClient.MVVM;

public class LoginViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<bool>? OnRequestRunningStateChanged;
    public event Action<string>? OnInvalidData;

    private readonly IDeviceIDProvider _deviceIDProvider;
    private readonly IAuthService _authService;
    private readonly IRateLimiter _rateLimiter;

    private readonly Logger _logger;
    private bool _isRequestRunning;

    public ILocalizationProvider Loc { get; }
    public ICommand LoginCommand { get; }
    public ICommand SwitchToRegisterCommand { get; }

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

    public LoginViewModel(
        IAuthService authService, 
        ICursorService cursorService, 
        IDeviceIDProvider deviceIDProvider, 
        Logger logger)
    {
        Loc = new LocalizationProvider(resourceName: ResourceBasePaths.Login, culture: null);
        LoginCommand = new RelayCommand(async (_) => await LoginAsync(), CanLogin);
        SwitchToRegisterCommand = new RelayCommand(async (_) => new RegisterWindow().SwitchTo());

        LoginBtnEnabled = true;
        
        Email = string.Empty;
        Password = string.Empty;

        _deviceIDProvider = deviceIDProvider;
        _rateLimiter = RateLimiters.Login;
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
                cursorService.SetCursor(Cursors.Wait);
            }
            else
            {
                cursorService.ResetCursor();
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
        LoginResult loginResult = await _authService.LoginAsync(new LoginRequest() 
        {
            Email = Email, 
            Password = Password ,
            DeviceId = _deviceIDProvider.GetDeviceID().ToString(),
        });
        //TODO: Handle loginResult (success/failure)

        OnRequestRunningStateChanged?.Invoke(false);
    }

    private bool CanLogin(object? _)
        => !_isRequestRunning;

    private async Task<bool> ValidateDataAsync()
    {
        _logger.LogInformation(LoggerParams.None, () => "Checking if login is possible");

        if (string.IsNullOrEmpty(Email) || !await NetworkHelper.IsEmailValidAsync(Email))
        {
            const string ErrorMsg = "The entered email is invalid";
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
