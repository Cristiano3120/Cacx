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
using CacxShared.Services;
using Cristiano3120.Logging;
using DotNetEnv;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using LogLevel = Cristiano3120.Logging.LogLevel;

namespace CacxServer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        SnowflakeGenerator snowflakeGenerator = new SnowflakeGenerator(1);
        int maxIdsPerMs = snowflakeGenerator.TestGeneratorOutput(iterations: 10000);
        Console.WriteLine($"Max amount of ids generated in a ms: {maxIdsPerMs}");

        _ = Env.Load();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        PathProvider pathProvider = new();

        _ = builder.Services.AddSingleton(_ => new JsonSerializerOptions()
        {
            WriteIndented = true,
        });

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
                Password = Env.GetString(key: "REDIS_PASSWORD"),
                AbortOnConnectFail = false,
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

        _ = app.MapControllers();
        _ = app.Use(async (context, next) =>
        {
            JsonSerializerOptions options = context.RequestServices.GetRequiredService<JsonSerializerOptions>();
            Logger logger = context.RequestServices.GetRequiredService<Logger>();

            context.Request.EnableBuffering();

            string requestBody = string.Empty;

            if (context.Request.ContentLength > 0 &&
                context.Request.Body.CanRead)
            {
                using StreamReader reader = new(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);

                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            logger.LogInformation(LoggerParams.None, () => $"REQUEST: {requestBody}");

            Stream originalBody = context.Response.Body;
            await using MemoryStream responseBody = new();
            context.Response.Body = responseBody;

            try
            {
                await next();
            }
            finally
            {
                responseBody.Position = 0;
                string responseText = await new StreamReader(responseBody).ReadToEndAsync();
                if (!string.IsNullOrEmpty(responseText))
                {
                    using JsonDocument doc = JsonDocument.Parse(responseText);

                    logger.LogInformation(LoggerParams.None, () => $"RESPONSE HEADERS: {JsonSerializer.Serialize(context.Response.Headers, options)}");

                    logger.LogInformation(LoggerParams.None, () => $"RESPONSE BODY: {JsonSerializer.Serialize(doc, options)}");

                    responseBody.Position = 0;
                    await responseBody.CopyToAsync(originalBody);
                    context.Response.Body = originalBody;
                }
            }
        });

        _ = app.UseStatusCodePages(async context =>
        {
            HttpResponse response = context.HttpContext.Response;

            if (response.StatusCode == StatusCodes.Status404NotFound)
            {
                response.ContentType = "application/json";
                await response.WriteAsJsonAsync(new ApiError
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Invalid path!"
                });
            }
        });

        await app.RunAsync(url: app.Configuration.GetValue<string>(key: "WebAdress"));
    }
}