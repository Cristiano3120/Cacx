using CacxClient.Extensions;
using CacxClient.MVVM;
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
        TosHyperlink.EnableHoverAnimation();
        TosCheckBox.EnableHoverAnimation();
        GoBackBtn.EnableHoverAnimation();

        EmailTextBox.InnerTextBox.DisableEmojiInput();
        PasswordTextBox.InnerTextBox.DisableEmojiInput();
        UsernameTextBox.InnerTextBox.DisableEmojiInput();
        DisplayNameTextBox.InnerTextBox.DisableEmojiInput();

        registerViewModel.OnDisplayInformation += DisplayInformation;
    }

    public void DisplayInformation(string msg, Color color)
        => InformationTextBlock.TriggerDisplayAnimation(color, msg);
}
