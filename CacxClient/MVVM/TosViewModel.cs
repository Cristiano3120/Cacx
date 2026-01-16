using Cacx.LocalizationManager.Abstractions;
using Cacx.LocalizationManager.Core;
using CacxClient.Commands;
using CacxClient.Extensions;
using CacxClient.Resources;
using CacxClient.Windows;
using System.Windows.Input;

namespace CacxClient.MVVM;

public sealed class TosViewModel
{
    public ILocalizationProvider Loc { get; }
    public ICommand GoBackCommand { get; } 

    public TosViewModel(RegisterViewModel registerViewModel)
    {
        Loc = new LocalizationProvider(resourceName: ResourceBasePaths.TOS, culture: null);
        GoBackCommand = new RelayCommand(_ => new RegisterWindow(registerViewModel).SwitchTo(resourceBasePath: ResourceBasePaths.Register));
    }
}
