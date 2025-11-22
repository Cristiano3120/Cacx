using CacxClient.Extensions;
using CacxClient.MVVM;
using Microsoft.VisualBasic;
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
        LoginBtn.EnableHoverAnimation();

        _ = Task.Run(async () =>
        {
            await Task.Delay(5000);
            Application.Current.Resources.MergedDictionaries[0]["HoverColor"] = Colors.Pink;
        });
    }
}
