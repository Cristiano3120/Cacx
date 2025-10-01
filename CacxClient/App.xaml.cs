using CacxClient.Communication.HTTPCommunication;
using CacxClient.Windows;
using CacxShared.Helper;
using Cristiano3120.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using System.Windows;

namespace CacxClient;

/// <summary>
/// <c>App:</c> <br></br>
/// Starting point of the application.
/// </summary>
public partial class App : Application
{
    public static ServiceProvider ServiceProvider { get; private set; } = default!;

    [LibraryImport("kernel32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AllocConsole();

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _ = AllocConsole();

        LoggerSettings loggerSettings = new()
        {
            LogLevel = LogLevel.Debug,
            MaxAmmountOfLoggingFiles = 3,
            PathToLogDirectory = SharedHelper.GetDynamicPath("Logs")
        };
        Logger logger = new(loggerSettings);

        Http http = new(logger);

        ServiceCollection services = new();

        _ = services.AddSingleton(logger);
        _ = services.AddSingleton(http);

        ServiceProvider = services.BuildServiceProvider();

        //Give the server some time to start cause I am starting both projects at the same time
        LoginWindow loginWindow = new();
        loginWindow.Show();
        Current.MainWindow = loginWindow;

        await Task.Delay(1000);
        //TODO: GitHub Issue #9
    }
}

