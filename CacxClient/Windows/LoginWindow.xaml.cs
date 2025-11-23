using CacxClient.Extensions;
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
        DataContext = _loginViewModel;

        LoginBtn.EnableHoverAnimation();
        _loginViewModel.OnInvalidData += DisplayInformation;
    }

    public void DisplayInformation(string msg)
    {
        Color color = (Color)Application.Current.Resources.MergedDictionaries[0]["TextErrorColor"]; //TODO: Don´t Hardcode!
        InformationTextBlock.TriggerDisplayAnimation(color, msg);
    }
}
