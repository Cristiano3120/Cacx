using CacxServer.Abstractions;
using CacxServer.Abstractions.Auth;
using CacxServer.Data.Redis;
using CacxServer.Data.Redis.Abstractions;
using CacxServer.Services;
using CacxShared.Abstractions;
using CacxShared.Services;
using Cristiano3120.Logging;
using DotNetEnv;
using StackExchange.Redis;
using LogLevel = Cristiano3120.Logging.LogLevel;

namespace CacxServer;

public class Program
{
    protected Program() { }

    public static void Main(string[] args)
    {
        _ = Env.Load();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        PathProvider pathProvider = new();

        _ = builder.Services.AddSingleton<IVerificationTokenService, VerificationTokenService>();
        _ = builder.Services.AddSingleton<INotificationService, NotificationService>();
        _ = builder.Services.AddScoped<IAuthService, AuthService>();
        _ = builder.Services.AddSingleton<IPathProvider, PathProvider>((_) => pathProvider);
        _ = builder.Services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>((_) =>
        {
            ConfigurationOptions conf = new()
            {
                EndPoints = { "localhost:6379" },
                Password = Env.GetString("REDIS_PASSWORD") 
            };

            return ConnectionMultiplexer.Connect(conf);
        });
        _ = builder.Services.AddScoped<IAuthRedisService, AuthRedisService>();
        _ = builder.Services.AddSingleton((serviceProvider) =>
        {
            LoggerSettings settings = new()
            {
                LogLevel = LogLevel.Debug,
                PathToLogDirectory = pathProvider.GetPath("Logs"),
            };

            return new Logger(settings);
        });

        // Add services to the container.

        _ = builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        _ = builder.Services.AddOpenApi();

        WebApplication app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            _ = app.MapOpenApi();
        }

        _ = app.UseHttpsRedirection();

        _ = app.UseAuthorization();

        _ = app.MapControllers();

        app.Run();
    }
}

