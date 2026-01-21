using CacxClient.Extensions;
using CacxClient.MVVM;
using CacxClient.Resources;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CacxClient.Windows;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : UserControl
{
    public LoginWindow()
    {
        InitializeComponent();

        LoginBtn.EnableHoverAnimation();
        CreateAccHyperlink.EnableHoverAnimation();
        EmailTextBox.InnerTextBox.DisableEmojiInput();
        PasswordTextBox.InnerTextBox.DisableEmojiInput();

        ((LoginViewModel)DataContext).OnInvalidData += DisplayInformation;
    }

    public void DisplayInformation(string msg)
        => InformationTextBlock.TriggerDisplayAnimation(ColorResources.TextErrorColor, msg);  
}
