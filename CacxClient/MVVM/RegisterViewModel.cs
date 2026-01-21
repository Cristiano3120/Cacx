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
using CacxShared.Abstractions;
using CacxClient.Abstractions.Auth;
using CacxClient.Windows;
using CacxClient.Extensions;
using Microsoft.Extensions.DependencyInjection;
using CacxClient.Resources;
using Cacx.LocalizationManager.Abstractions;
using Cacx.LocalizationManager.Core;

namespace CacxClient.MVVM;

public class RegisterViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<string, Color>? OnDisplayInformation;
    public event Action<bool>? OnRequestRunningStateChanged;
    public ILocalizationProvider Loc { get; }
    public ICommand GeneratePasswordCommand { get; }
    public ICommand RegisterCommand { get; }
    public ICommand OpenTOSCommand { get; }
    public ICommand GoBackCommand { get; }

    private readonly INavigationService _navigationService;
    private readonly IDeviceIDProvider _deviceIDProvider;
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

    public bool TOSAccepted
    {
        get => field;
        set
        {
            field = value;
            OnPropertyChanged(nameof(TOSAccepted));
        }
    }

    public RegisterViewModel(
        INavigationService navigationService,
        IDeviceIDProvider deviceIDProvider, 
        ICursorService cursorService, 
        IAuthService authService)
    {
        Loc = new LocalizationProvider(resourceName: ResourceBasePaths.Register, culture: null);
        RegisterCommand = new RelayCommand(async (_) => await RegisterAsync(), CanRegister);
        OpenTOSCommand = new RelayCommand((_) => new TOSWindow(this).SwitchTo());
        GoBackCommand = new RelayCommand(_ => navigationService.NavigateToLogin());
        GeneratePasswordCommand = new RelayCommand(async (_) =>
        {
            Password = new PasswordGenerator().GeneratePassword(20);
            Clipboard.SetText(Password);

            Color? color = ColorResources.TextPrimaryColor;
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

        _navigationService = navigationService;
        _deviceIDProvider = deviceIDProvider;
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
        if (!TOSAccepted)
        {
            const string ErrorMsg = "You must accept the Terms of Service to register";
            OnDisplayInformation?.Invoke(ErrorMsg, ColorResources.TextErrorColor);
            return;
        }

        if (!_rateLimiter.TryConsume())
        {
            const string ErrorMsg = "Don´t spam :( You gotta wait a bit!";
            OnDisplayInformation?.Invoke(ErrorMsg, ColorResources.TextErrorColor);
            return;
        }

        if (! await ValidateDataAsync())
            return;
        
        OnRequestRunningStateChanged?.Invoke(true);
        RegisterResult result = await _authService.RegisterAsync(new RegisterRequest()
        {
            Email = Email,
            Username = Username,
            Password = Password,
            DisplayName = DisplayName,
            DeviceId = _deviceIDProvider.GetDeviceID().ToString(),
        });
        OnRequestRunningStateChanged?.Invoke(false);

        if (!result.IsSuccess)
        {
            OnDisplayInformation?.Invoke(result.ErrorMessage!, ColorResources.TextErrorColor);
            return;
        }

        _navigationService.NavigateToVerification(token: result.Token);
    }

    private async Task<bool> ValidateDataAsync()
    {
        if (string.IsNullOrEmpty(Email) || !await NetworkHelper.IsEmailValidAsync(Email))
        {
            const string ErrorMsg = "The entered email is invalid";
            OnDisplayInformation?.Invoke(ErrorMsg, ColorResources.TextErrorColor);

            return false;
        }

        const byte MinPasswordLength = 8;
        if (string.IsNullOrEmpty(Email) || Password.Length < MinPasswordLength)
        {
            const string ErrorMsg = "Password must be at least 8 characters long";
            OnDisplayInformation?.Invoke(ErrorMsg, ColorResources.TextErrorColor);
            return false;
        }

        if (string.IsNullOrEmpty(Username) || Username.Any(x => !char.IsLetterOrDigit(x) && x != '_' && x != '-'))
        {
            const string ErrorMsg = "Username can only contain letters, digits, underscores and hyphens";
            OnDisplayInformation?.Invoke(ErrorMsg, ColorResources.TextErrorColor);
            return false;
        }

        if (string.IsNullOrEmpty(DisplayName))
        {
            const string ErrorMsg = "Display name cannot be empty";
            OnDisplayInformation?.Invoke(ErrorMsg, ColorResources.TextErrorColor);
            return false;
        }

        return true;
    }

    public bool CanRegister(object? sender)
        => !_isRequestRunning;

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
