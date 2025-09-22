using CacxClient.HTTPCommunication;
using CacxShared;
using Cristiano3120.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using System.Text.Json;
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

    protected override void OnStartup(StartupEventArgs e)
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

        ServiceCollection services = new();

        _ = services.AddSingleton(logger);
        _ = services.AddSingleton<Http>();
        ServiceProvider = services.BuildServiceProvider();
    }
}

