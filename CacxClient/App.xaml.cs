using Cacx.LanguageManager.Abstractions;
using Cacx.LanguageManager.Core;
using Cacx.LanguageManager.Wpf;
using CacxClient.Helper;
using CacxClient.Interfaces;
using CacxClient.MVVM;
using CacxClient.Services;
using CacxClient.Services.RateLimiter;
using CacxClient.Windows;
using Cristiano3120.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

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
        SetupExceptionHandling();
        _ = AllocConsole();

        string PathToAppsettings = GetPathToAppsettings();
        AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    _ = config.SetBasePath(AppContext.BaseDirectory);
                    _ = config.AddJsonFile(path: PathToAppsettings, optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
#pragma warning disable CA1416
                    _ = services.AddSingleton<ILocalizationService, LocalizationService>();
                    _ = services.AddSingleton<ICursorService, CursorService>();
                    _ = services.AddSingleton<ITokenProvider, TokenProvider>();
                    _ = services.AddSingleton<IAuthService, AuthService>();
                    _ = services.AddSingleton<RateLimiters>();
                    _ = services.AddSingleton<IHttp, Http>();
                    _ = services.AddSingleton<MainWindow>();
                    _ = services.AddSingleton((serviceProvider) =>
                    {
                        LoggerSettings loggerSettings = new()
                        {
                            LogLevel = LogLevel.Debug,
                        };

                        return new Logger(loggerSettings);
                    });

                    _ = services.AddTransient<LoginViewModel>();

                    LocalizationProvider.Service = new LocalizationService(basePath: "CacxClient.Resources.Login.Login");
                    LocalizationProvider.Service.SetLanguage(new System.Globalization.CultureInfo("en-US"));
                }).Build();
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
    }

    private static string GetPathToAppsettings()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!Debugger.IsAttached)
        {
            // .exe is running
            return path;
        }
        
        int indexOfBin = path.IndexOf(@"\bin", StringComparison.OrdinalIgnoreCase);
        if (indexOfBin >= 0)
        {
            path = path[..indexOfBin] + @"\appsettings.json";
        }

        return path;
    }

    private void SetupExceptionHandling()
    {
        string logFilePath = Path.Combine(AppContext.BaseDirectory, "UnhandledExceptions.log");

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {e.ExceptionObject}{Environment.NewLine}\n");
        };

        DispatcherUnhandledException += (s, e) =>
        {
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {e.Exception}{Environment.NewLine}\n");
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {e.Exception}{Environment.NewLine}\n");
        };
    }
}
