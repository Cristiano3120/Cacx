using CacxClient.Abstractions;
using CacxClient.Services.RateLimiter;
using Cristiano3120.Logging;
using System.ComponentModel;
using System.Windows.Input;

namespace CacxClient.MVVM;

internal class RegisterViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public ICommand RegisterCommand { get; }

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
        _rateLimiter = RateLimiters.Register;
        _cursorService = cursorService;
        _authService = authService;
        _logger = logger;

        Email = string.Empty;
        Username = string.Empty;
        DisplayName = string.Empty;
        Password = string.Empty;
    }

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
