using CacxClient.MVVM;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CacxClient.Windows;
/// <summary>
/// Interaction logic for RegisterWindow.xaml
/// </summary>
public partial class RegisterWindow : UserControl
{
    public RegisterWindow(RegisterViewModel registerViewModel)
    {
        InitializeComponent();
        DataContext = registerViewModel;
    }

    public RegisterWindow()
    {
        InitializeComponent();
        DataContext = App.AppHost.Services.GetRequiredService<RegisterViewModel>();
    }
}
