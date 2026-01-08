using CacxClient.Commands;
using CacxClient.Extensions;
using CacxClient.Resources;
using CacxClient.Windows;
using System.Windows.Input;

namespace CacxClient.MVVM;

public sealed class TosViewModel(RegisterViewModel registerViewModel)
{
    public ICommand GoBackCommand { get; } 
        = new RelayCommand(_ => new RegisterWindow(registerViewModel).SwitchTo(resourceBasePath: ResourceBasePaths.Register));
}
