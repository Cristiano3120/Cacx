using System.Windows;

namespace CacxClient.Windows;
/// <summary>
/// Interaction logic for CreateAccPart2Window.xaml
/// </summary>
public partial class CreateAccPart2Window : Window
{
    public CreateAccPart2Window() 
        => InitializeComponent();

    public CreateAccPart2Window(string email, string password, DateOnly birthday)
    {
        InitializeComponent();
    }
}
