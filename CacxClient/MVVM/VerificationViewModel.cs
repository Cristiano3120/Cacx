using Cacx.LocalizationManager.Abstractions;
using Cacx.LocalizationManager.Core;
using CacxClient.Abstractions;
using CacxClient.Abstractions.Auth;
using CacxClient.Commands;
using CacxClient.Resources;
using CacxClient.Services;
using System.Windows.Input;

namespace CacxClient.MVVM;

public sealed class VerificationViewModel
{
    public ILocalizationProvider Loc { get; }
    public ICommand RequestEmail { get; }
    public ICommand Verify { get; }

    private readonly INavigationService _navigationService;
    private readonly IAuthService _authService;
    public VerificationViewModel(INavigationService navigationService, IAuthService authService)
    {
        Loc = new LocalizationProvider(resourceName: ResourceBasePaths.Verification, culture: null);
        RequestEmail = new RelayCommand(execute: async (_) => await RequestVerificationEmailAsync());

        _navigationService = navigationService;
        _authService = authService;
    }

    public VerificationViewModel() { }

    private async Task RequestVerificationEmailAsync()
    {
        RequestVerificationEmailResult result = await _authService.RequestVerificationEmailAsync();
        if (result.IsSuccess)
        {
            DisplayInformation(Loc.GetString("VerificationEmailSentMessage"));
            return;
        }

        //RESTART
        if (result.SessionExpired)
        {
            DisplayInformation(result.ErrorMessage);
            await Task.Delay(millisecondsDelay: 2500);

            _navigationService.NavigateToLogin();
            return;
        }

        //On Cooldown
        DisplayInformation(result.ErrorMessage);
    }

    private void DisplayInformation(string message)
    {
    }
}
