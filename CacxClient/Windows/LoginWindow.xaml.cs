using CacxClient.MVVM;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CacxClient.Windows;
/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : UserControl
{
    private readonly LoginViewModel _loginViewModel;

    public LoginWindow(LoginViewModel loginViewModel)
    {
        InitializeComponent();
        _loginViewModel = loginViewModel;
    }
}
