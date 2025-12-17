using Cacx.LanguageManager.Abstractions;
using Cacx.LanguageManager.Core;
using CacxClient.Abstractions;
using CacxClient.MVVM;
using CacxClient.Services;
using CacxClient.Services.RateLimiter;
using CacxClient.Windows;
using Cristiano3120.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace CacxClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost AppHost { get; private set; } = default!;

    [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int AllocConsole();

    [DllImport("kernel32.dll", EntryPoint = "FreeConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int FreeConsole();

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
#pragma warning disable CA1416
                    JsonSerializerOptions jsonSerializerOptions = new()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true,
                    };

                    _ = services.AddSingleton<ILocalizationService, LocalizationService>((_) => new LocalizationService(basePath: "CacxClient.Resources.Login.Login"));
                    _ = services.AddSingleton((_) => new ThemeManager(pathProvider, jsonSerializerOptions));
                    _ = services.AddSingleton<ICursorService, CursorService>();
                    _ = services.AddSingleton<ITokenProvider, TokenProvider>();
                    _ = services.AddSingleton<IPathProvider, PathProvider>();
                    _ = services.AddSingleton<IAuthService, AuthService>();
                    _ = services.AddSingleton(jsonSerializerOptions);
                    _ = services.AddSingleton<IHttp, Http>();
                    _ = services.AddSingleton<MainWindow>();
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

        _ = AppHost.Services.GetRequiredService<ILocalizationService>();
    }
#pragma warning restore CA1416

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

        //For Demo purposes only...
        CultureInfo cultureInfo = new("en-US");
        provider.GetRequiredService<ILocalizationService>().SetLanguage(cultureInfo); //language switch

        ThemeManager themeManager = provider.GetRequiredService<ThemeManager>();
        themeManager.CreateThemeTest();
        themeManager.SetToLightMode(); //theme switch
    }

    /// <summary>
    /// To Do: Rework
    /// This is not a beautiful implementation, but it works.
    /// I´m only doing this because I´m leaving this project behind and just need to quickly switch between windows
    /// for demo purposes.
    /// </summary>
    public static void SwitchWindow(UserControl window)
    {
        AppHost.Services.GetRequiredService<MainWindow>().Content = window;
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
