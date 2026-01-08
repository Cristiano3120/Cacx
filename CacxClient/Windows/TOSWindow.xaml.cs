using CacxClient.Extensions;
using CacxClient.MVVM;
using System.Windows.Controls;

namespace CacxClient.Windows;

/// <summary>
/// Interaction logic for TOSWindow.xaml
/// </summary>
public partial class TOSWindow : UserControl
{
    public TOSWindow(RegisterViewModel registerViewModel)
    {
        InitializeComponent();
        DataContext = new TosViewModel(registerViewModel);
        GoBackBtn.EnableHoverAnimation();
    }
}
