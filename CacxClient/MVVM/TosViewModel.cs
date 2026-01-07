using CacxClient.Commands;
using CacxClient.Extensions;
using CacxClient.Resources;
using CacxClient.Windows;
using System.Windows.Input;

namespace CacxClient.MVVM;

public sealed class TosViewModel
{
    public ICommand GoBackCommand { get; }

    public TosViewModel() 
        => GoBackCommand = new RelayCommand(_ => new RegisterWindow().SwitchTo(resourceBasePath: ResourceBasePaths.Register));
}
