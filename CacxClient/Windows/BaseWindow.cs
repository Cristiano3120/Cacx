using Cristiano3120.Logging;
using System.Windows;

namespace CacxClient.Windows;

/// <summary>
/// Initiates a few Window properties
/// </summary>
public class BaseWindow : Window
{ 
    protected readonly Logger logger;

    public BaseWindow()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        logger = App.GetLogger();
    }
}
