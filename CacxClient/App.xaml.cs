using Cacx.LanguageManager.Abstractions;
using Cacx.LanguageManager.Core;
using CacxClient.Abstractions;
using CacxClient.Abstractions.Auth;
using CacxClient.MVVM;
using CacxClient.Services;
using CacxClient.Windows;
using CacxShared.Abstractions;
using CacxShared.Services;
using Cristiano3120.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;

namespace CacxClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost AppHost { get; private set; } = default!;

    [LibraryImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true)]
    private static partial int AllocConsole();

    [LibraryImport("kernel32.dll", EntryPoint = "FreeConsole", SetLastError = true)]
    private static partial int FreeConsole();

    public App()
    {
        PathProvider pathProvider = new();
        SetupExceptionHandling(pathProvider);
        _ = AllocConsole();

        string PathToAppsettings = pathProvider.GetPath(relativePath: "appsettings.json");
        AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    _ = config.SetBasePath(AppContext.BaseDirectory);
                    _ = config.AddJsonFile(path: PathToAppsettings, optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    _ = services.AddSingleton<ILocalizationService, LocalizationService>(_ => new(basePath: "CacxClient.Resources.Login.Login"));
                    _ = services.AddSingleton<ICursorService, CursorService>();
                    _ = services.AddSingleton<ITokenProvider, TokenProvider>();
                    _ = services.AddSingleton<IPathProvider, PathProvider>();
                    _ = services.AddSingleton<IAuthService, AuthService>();
                    _ = services.AddSingleton<ThemeManager>();
                    _ = services.AddSingleton<IHttp, Http>();
                    _ = services.AddSingleton<MainWindow>();
                    _ = services.AddSingleton(pathProvider);

                    _ = services.AddSingleton<JsonSerializerOptions>(_ => new()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true,
                    });
                    _ = services.AddSingleton((_) =>
                    {
                        LoggerSettings loggerSettings = new()
                        {
                            LogLevel = LogLevel.Debug,
                        };

                        return new Logger(loggerSettings);
                    });

                    _ = services.AddTransient<RegisterViewModel>();
                    _ = services.AddTransient<LoginViewModel>();
                }).Build();

        _ = AppHost.Services.GetRequiredService<ILocalizationService>(); //Init LocalizationService
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IServiceProvider serviceProvider = AppHost.Services;
        InitMainWindow(serviceProvider);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        _ = FreeConsole();
        base.OnExit(e);
    }

    private static void InitMainWindow(IServiceProvider provider)
    {
        MainWindow mainWindow = provider.GetRequiredService<MainWindow>();
        mainWindow.Content = new LoginWindow(provider.GetRequiredService<LoginViewModel>());
        mainWindow.Show();
    }

    private void SetupExceptionHandling(IPathProvider pathProvider)
    {
        const string FileName = "UnhandledExceptions.log";
        string logFilePath = pathProvider.GetPath(FileName);

        AppDomain.CurrentDomain.UnhandledException += (s, e)
            => File.AppendAllText(logFilePath, $"{DateTime.Now}: {e.ExceptionObject}{Environment.NewLine}\n");

        DispatcherUnhandledException += (s, e)
            => File.AppendAllText(logFilePath, $"{DateTime.Now}: {e.Exception}{Environment.NewLine}\n");

        TaskScheduler.UnobservedTaskException += (s, e)
            => File.AppendAllText(logFilePath, $"{DateTime.Now}: {e.Exception}{Environment.NewLine}\n");
    }
}
