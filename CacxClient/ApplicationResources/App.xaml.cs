﻿using CacxClient.Communication.HTTPCommunication;
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
            PathToLogDirectory = SharedHelper.GetDynamicPath("Logs")
        };
        Logger logger = new(loggerSettings);

        Http http = new(logger);

        ServiceCollection services = new();

        _ = services.AddSingleton(logger);
        _ = services.AddSingleton(http);

        // Fix for S2696: Move static field assignment to a static method
        InitializeServiceProvider(services);

        LoginWindow loginWindow = new();
        loginWindow.Show();
        Current.MainWindow = loginWindow;

        //Give the server some time to start cause I am starting both projects at the same time
        await Task.Delay(1000);
    }

    private static void InitializeServiceProvider(ServiceCollection services)
    {
        _serviceProvider = services.BuildServiceProvider();
    }

    public static Logger GetLogger() => _serviceProvider.GetRequiredService<Logger>();
    public static Http GetHttp() => _serviceProvider.GetRequiredService<Http>();
}

