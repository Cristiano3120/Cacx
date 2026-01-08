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
    public LoginWindow(LoginViewModel loginViewModel)
    {
        InitializeComponent();
        DataContext = loginViewModel;

        LoginBtn.EnableHoverAnimation();
        CreateAccHyperlink.EnableHoverAnimation();

        loginViewModel.OnInvalidData += DisplayInformation;
    }

    public LoginWindow() : this(App.AppHost.Services.GetRequiredService<LoginViewModel>()) { }

    public void DisplayInformation(string msg)
        => InformationTextBlock.TriggerDisplayAnimation(ColorResources.TextErrorColor, msg);  
}
