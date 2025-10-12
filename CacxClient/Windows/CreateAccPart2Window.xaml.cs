using Cristiano3120.Logging;

namespace CacxClient.Windows;
/// <summary>
/// Interaction logic for CreateAccPart2Window.xaml
/// </summary>
public partial class CreateAccPart2Window : BaseWindow
{
    public CreateAccPart2Window() 
        => InitializeComponent();

    public CreateAccPart2Window(string email, string password, DateOnly birthday)
    {
        logger.LogDebug(LoggerParams.None, $"{nameof(CreateAccPart2Window)} initialized");
        InitializeComponent();
    }
}
