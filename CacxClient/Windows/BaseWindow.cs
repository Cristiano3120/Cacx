using CacxClient.Communication.HTTPCommunication;
using Cristiano3120.Logging;
using System.Windows;

namespace CacxClient.Windows;

/// <summary>
/// Initiates a few Window properties
/// </summary>
public class BaseWindow : Window
{
    protected readonly Logger logger;
    protected readonly Http http;

    public BaseWindow()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        logger = App.GetLogger();
        http = App.GetHttp();
    }
}
