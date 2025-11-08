using CacxClient.Communication.HTTPCommunication;
using CacxClient.Helpers;
using CacxClient.Windows;
using CacxShared.Endpoints;
using CacxShared.Helper;
using CacxShared.SharedDTOs;
using CacxShared.SharedMinioResources;
using Cristiano3120.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;

namespace CacxClient;

/// <summary>
/// <c>App:</c> <br></br>
/// Starting point of the application.
/// </summary>
public partial class App : Application
{
    private static ServiceProvider _serviceProvider = default!;

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
            PathToLogDirectory = SharedHelper.GetDynamicPath(CacxShared.Project.CacxClient, "Logs")
        };
        Logger logger = new(loggerSettings);

        Http http = new(logger);

        ServiceCollection services = new();

        _ = services.AddSingleton(logger);
        _ = services.AddSingleton(http);

        // Fix for S2696: Move static field assignment to a static method
        InitializeServiceProvider(services);

        //GuiHelper.SwitchWindow<LoginWindow>();
        LoginWindow loginWindow = new();
        loginWindow.Show();
        Current.MainWindow = loginWindow;
    }

    private static void InitializeServiceProvider(ServiceCollection services)
    {
        _serviceProvider = services.BuildServiceProvider();
    }

    public static Logger GetLogger() => _serviceProvider.GetRequiredService<Logger>();
    public static Http GetHttp() => _serviceProvider.GetRequiredService<Http>();
}

