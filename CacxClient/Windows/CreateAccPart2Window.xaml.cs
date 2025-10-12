using Cristiano3120.Logging;
using CacxShared.SharedDTOs;

namespace CacxClient.Windows;
/// <summary>
/// Interaction logic for CreateAccPart2Window.xaml
/// </summary>
public partial class CreateAccPart2Window : BaseWindow
{
    public CreateAccPart2Window() 
        => InitializeComponent();

    public CreateAccPart2Window(User user)
    {
        logger.LogDebug(LoggerParams.None, $"{nameof(CreateAccPart2Window)} initialized");
        InitializeComponent();
    }
}
