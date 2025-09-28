using System.Windows;

namespace CacxClient.Windows;

/// <summary>
/// Initiates a few Window properties
/// </summary>
public class BaseWindow : Window
{ 
    public BaseWindow()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }
}
