using CacxClient.Extensions;
using CacxClient.MVVM;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using System.Windows.Media;

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

        RegisterBtn.EnableHoverAnimation();
        RandomPasswordBtn.EnableHoverAnimation();

        registerViewModel.OnDisplayInformation += DisplayInformation;
    }

    public RegisterWindow() : this(App.AppHost.Services.GetRequiredService<RegisterViewModel>()) { }

    public void DisplayInformation(string msg, Color color)
        => InformationTextBlock.TriggerDisplayAnimation(color, msg);
}
