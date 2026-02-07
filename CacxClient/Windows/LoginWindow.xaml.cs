using CacxClient.Abstractions;
using CacxClient.Extensions;
using CacxClient.MVVM;
using CacxClient.Resources;
using System.Windows.Controls;

namespace CacxClient.Windows;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : UserControl
{
    public LoginWindow(LoginViewModel loginViewModel)
    {
        InitializeComponent();
        DataContext = loginViewModel;

        LoginBtn.EnableHoverAnimation();
        CreateAccHyperlink.EnableHoverAnimation();
        EmailTextBox.InnerTextBox.DisableCertainChars(CharacterTypes.Emoji);
        PasswordTextBox.InnerTextBox.DisableCertainChars(CharacterTypes.Emoji);

        loginViewModel.OnInvalidData += DisplayInformation;
    }

    public void DisplayInformation(string msg)
        => InformationTextBlock.TriggerDisplayAnimation(ColorResources.TextErrorColor, msg);  
}
