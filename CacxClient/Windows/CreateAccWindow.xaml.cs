using CacxClient.PasswordGeneratorResources;
using System.Windows;

namespace CacxClient.Windows;

/// <summary>
/// Interaction logic for CreateAccWindow.xaml
/// </summary>
public partial class CreateAccWindow : BaseWindow
{
    public CreateAccWindow()
    {
        InitializeComponent();
        GeneratePwBtn.Click += GeneratePwBtn_Click;
    }

    private void GeneratePwBtn_Click(object sender, RoutedEventArgs e)
    {
        string password = PasswordTextBox.Text = PasswordGenerator.GeneratePassword();
        Clipboard.SetText(password); //TODO: GitHub Issue #10
        PasswordTextBox.Text = password;
    }
}
