using Cacx.LocalizationManager.Abstractions;
using Cacx.LocalizationManager.Core;
using CacxClient.Abstractions;
using CacxClient.Commands;
using CacxClient.Resources;
using System.Windows.Input;

namespace CacxClient.MVVM;

public sealed class TosViewModel
{
    public ILocalizationProvider Loc { get; }
    public ICommand GoBackCommand { get; }
    private RegisterViewModel _registerViewModel;

    public TosViewModel(INavigationService navigationService)
    {
        Loc = new LocalizationProvider(resourceName: ResourceBasePaths.TOS, culture: null);
        GoBackCommand = new RelayCommand(_ => navigationService.NavigateToRegister(_registerViewModel));
        _registerViewModel = default!;
    }

    public void Activate(RegisterViewModel registerViewModel)
    {
        _registerViewModel = registerViewModel;
    }
}
