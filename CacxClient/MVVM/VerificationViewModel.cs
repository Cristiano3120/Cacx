using Cacx.LocalizationManager.Abstractions;
using Cacx.LocalizationManager.Core;
using CacxClient.Abstractions.Auth;
using CacxClient.Commands;
using CacxClient.Resources;
using System.Windows.Input;

namespace CacxClient.MVVM;

public sealed class VerificationViewModel
{
    public ILocalizationProvider Loc { get; }
    public ICommand RequestEmail { get; }
    public ICommand Verify { get; }

    public VerificationViewModel(IAuthService authService)
    {
        Loc = new LocalizationProvider(resourceName: ResourceBasePaths.Verification, culture: null);
        RequestEmail = new RelayCommand(execute: async (_) => await authService.RequestVerificationEmailAsync());
    }

    public VerificationViewModel() { }
}
