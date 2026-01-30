using Cacx.LocalizationManager.Abstractions;
using Cacx.LocalizationManager.Core;
using CacxClient.Abstractions;
using CacxClient.Abstractions.Auth;
using CacxClient.Commands;
using CacxClient.Resources;
using System.Windows.Input;
using System.Windows.Media;

namespace CacxClient.MVVM;

public sealed class VerificationViewModel
{
    public ILocalizationProvider Loc { get; }
    public ICommand RequestEmail { get; }
    public ICommand Verify { get; }

    public event Action<string, Color>? OnDisplayInformation;
    private readonly INavigationService _navigationService;
    private readonly IRateLimiter _rateLimiter;
    private readonly IAuthService _authService;
    public VerificationViewModel(
        INavigationService navigationService, 
        IAuthService authService,
        IRateLimiter rateLimiter)
    {
        Loc = new LocalizationProvider(resourceName: ResourceBasePaths.Verification, culture: null);
        RequestEmail = new RelayCommand(execute: async (_) => await RequestVerificationEmailAsync());

        _navigationService = navigationService;
        _authService = authService;
        _rateLimiter = rateLimiter;
    }

    public VerificationViewModel() { }

    private async Task RequestVerificationEmailAsync()
    {
        RequestVerificationEmailResult result = await _authService.RequestVerificationEmailAsync();
        if (result.IsSuccess)
        {
            DisplayInformation(Loc.GetString("VerificationEmailSentMessage"), ColorResources.TextPrimaryColor);
            return;
        }

        //RESTART
        if (result.SessionExpired)
        {
            DisplayInformation(result.ErrorMessage, ColorResources.TextErrorColor);
            await Task.Delay(millisecondsDelay: 2500);

            _navigationService.NavigateToLogin();
            return;
        }

        //On Cooldown
        DisplayInformation(result.ErrorMessage, ColorResources.TextErrorColor);
    }

    private void DisplayInformation(string message, Color color)
        => OnDisplayInformation?.Invoke(message, color);
}
