using CacxServer.Abstractions;
using CacxServer.Abstractions.Auth;
using CacxServer.Data.PostgreSQL;
using CacxServer.Data.PostgreSQL.Abstractions;
using CacxServer.Data.PostgreSQL.Repositories;
using CacxServer.Data.Redis.Abstractions;
using CacxServer.Data.Redis.Services;
using CacxServer.RateLimiter.AuthRateLimiter.Abstractions;
using CacxServer.RateLimiter.AuthRateLimiter.Services;
using CacxServer.Security.Hashing.Abstractions;
using CacxServer.Security.Hashing.Services;
using CacxServer.Services;
using CacxShared.Abstractions;
using CacxShared.APIResponse;
using CacxShared.Services;
using Cristiano3120.Logging;
using DotNetEnv;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Net;
using LogLevel = Cristiano3120.Logging.LogLevel;

namespace CacxServer;

public static class Program
{
    public static void Main(string[] args)
    {
        _ = Env.Load();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        PathProvider pathProvider = new();

        _ = builder.Services.AddKeyedSingleton<IHashingService, Sha256HashingService>(HashingAlgorithm.Sha256);
        _ = builder.Services.AddKeyedSingleton<IHashingService, BCryptHashingService>(HashingAlgorithm.BCrypt);

        _ = builder.Services.AddSingleton<IVerificationTokenService, VerificationTokenService>();
        _ = builder.Services.AddSingleton<IPathProvider, PathProvider>(_ => pathProvider);
        _ = builder.Services.AddSingleton<INotificationService, NotificationService>();

        _ = builder.Services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>((_) =>
        {
            ConfigurationOptions conf = new()
            {
                EndPoints = { "localhost:6379" },
                Password = Env.GetString(key: "REDIS_PASSWORD")
            };

            return ConnectionMultiplexer.Connect(conf);
        });
        _ = builder.Services.AddSingleton((serviceProvider) =>
        {
            LoggerSettings settings = new()
            {
                LogLevel = LogLevel.Debug,
                PathToLogDirectory = pathProvider.GetPath("Logs"),
            };

            return new Logger(settings);
        });

        _ = builder.Services.AddScoped<IAuthRedisRateLimiter, AuthRedisRateLimiter>();
        _ = builder.Services.AddScoped<IAuthRedisService, AuthRedisService>();
        _ = builder.Services.AddScoped<IAuthRateLimiter, AuthRateLimiter>();
        _ = builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        _ = builder.Services.AddScoped<IAuthService, AuthService>();

        _ = builder.Services.AddDbContextPool<CacxDbContext>(opt => opt.UseNpgsql(
            connectionString: builder.Configuration.GetConnectionString("PostgreSQL")));

        _ = builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        _ = builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        _ = builder.Services.AddOpenApi();

        WebApplication app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            _ = app.MapOpenApi();
        }

        _ = app.UseForwardedHeaders();
        _ = app.UseHttpsRedirection();
        _ = app.UseAuthorization();
        _ = app.MapControllers();
        _ = app.Use(async (context, next) =>
        {
            await next();
            
            if (context.Response.StatusCode < 400)
            {
                return;
            }

            ApiError apiError = (HttpStatusCode)context.Response.StatusCode switch
            {
                HttpStatusCode.NotFound => new ApiError()
                {
                    StatusCode = (HttpStatusCode)context.Response.StatusCode,
                    Message = "Invalid path!"
                },

                _ => new ApiError()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "The Server encountered some unknown error. Try again!"
                }
            };

            await context.Response.WriteAsJsonAsync(apiError);
        });

        app.Run(url: app.Configuration.GetValue<string>(key: "WebAdress"));
    }
}

