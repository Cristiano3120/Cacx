using CacxClient.Abstractions;
using CacxClient.Extensions;
using CacxClient.MVVM;
using System.Windows.Controls;

namespace CacxClient.Windows;

/// <summary>
/// Interaction logic for VerificationWindow.xaml
/// </summary>
public partial class VerificationWindow : UserControl
{
    public VerificationWindow(VerificationViewModel verificationViewModel)
    {
        InitializeComponent();
        DataContext = verificationViewModel;

        RequestEmailHyperlink.EnableHoverAnimation();
        CodeTextBox.InnerTextBox.DisableCertainChars(CharacterTypes.Emoji | CharacterTypes.Text);
    }
}