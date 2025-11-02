using CacxServer.Helper;
using CacxServer.Services;
using CacxServer.Storage;
using CacxServer.UserDataDatabaseResources;
using CacxShared.ApiResources;
using CacxShared.Helper;
using Cristiano3120.Logging;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using System.Net;
using System.Text.Json;

namespace CacxServer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        _ = Env.Load();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        _ = builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        // https://localhost:5001/openapi/v1.json
        _ = builder.Services.AddOpenApi();

        //Configure Logger
        LoggerSettings loggerSettings = new()
        {
            LogLevel = Cristiano3120.Logging.LogLevel.Debug,
            MaxAmmountOfLoggingFiles = 3,
            PathToLogDirectory = SharedHelper.GetDynamicPath(CacxShared.Project.CacxServer, "Logs")
        };
        Logger logger = new(loggerSettings);
        _ = builder.Services.AddSingleton(logger);

        _ = builder.Services.AddSingleton<AuthService>();

        //Get the Url depending on the environment
        string url = (builder.Configuration.GetValue<bool>(key: "Testing")
            ? builder.Configuration.GetValue<string>("TestUrl")
            : builder.Configuration.GetValue<string>("Url"))
            ?? throw new InvalidOperationException("The appSettings.json file is broken");

        ushort workerId = builder.Configuration.GetValue<ushort>(key: "workerId");
        SnowflakeGenerator.Initialize(workerId);

        //Configure JsonSerializerOptions
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        _ = builder.Services.AddSingleton(jsonSerializerOptions);

        _ = builder.Services.AddSingleton<LoggingHelper>();

        _ = builder.Services.AddSingleton<ObjectStorageManager>();

        string postgreConnStr = builder.Configuration["UserDataDbConnString"]
            ?? throw new NotImplementedException("Conn str missing");

        _ = builder.Services.AddDbContext<UserDataDbContext>((optionsBuilder) =>
        {
            _ = optionsBuilder.UseNpgsql(postgreConnStr);
        });

        _ = builder.Services.AddSingleton(sp =>
        {
            Logger logger = sp.GetRequiredService<Logger>();
            IServiceScopeFactory scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            return new UserDataDatabase(logger, postgreConnStr, scopeFactory);
        });


        string? minioEndpoint = builder.Configuration.GetValue<string>(key: nameof(minioEndpoint));
        string? minioAccessKey = builder.Configuration.GetValue<string>(key: nameof(minioAccessKey));
        string? minioPassword = builder.Configuration.GetValue<string>(key: nameof(minioPassword));

        if (minioEndpoint is null || minioAccessKey is null || minioPassword is null)
        {
            CallerInfos callerInfos = CallerInfos.Create();
            throw new InvalidOperationException($"One of the minio credentials is null: [File: {callerInfos.FilePath}] [Line: {callerInfos.LineNum}]");
        }

        _ = builder.Services.AddMinio((configureClient) => configureClient
            .WithEndpoint(minioEndpoint)
            .WithCredentials(minioAccessKey, minioPassword) 
            .WithSSL(secure: false)
            .Build());


        _ = builder.Services.AddSingleton<ObjectStorageManager>();

        //Configure a logging when a request comes in and is done executing 
        _ = builder.Services.AddControllers(options => _ = options.Filters.Add<GlobalActionFilter>()).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;
        });

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            _ = app.MapOpenApi();
        }

        _ = app.UseHttpsRedirection();
        _ = app.UseAuthorization();
        _ = app.MapControllers();

        //Warmups
        using (IServiceScope scope = app.Services.CreateScope())
        {
            UserDataDatabase db = scope.ServiceProvider.GetRequiredService<UserDataDatabase>();
            await db.WarmupAsync();
        }

        await SharedCryptographyHelper.WarmupAsync();
        CryptographyHelper.Warmup();

        //Handle errors!
        _ = app.Use(async (context, next) =>
        {
            await next();

            //Request was successful
            if (context.Response.StatusCode < 400)
            {
                return;
            }

            HttpStatusCode statusCode = (HttpStatusCode)context.Response.StatusCode;

            //Clear body data
            await context.Response.Body.WriteAsync(new ReadOnlyMemory<byte>());

            if (statusCode == HttpStatusCode.NotFound)
            {
                string requestRoute = $"[{statusCode}]: INVALID PATH: [{context.Request.Method}]: {context.Request.Path}";
                logger.LogError(LoggerParams.None, requestRoute);
            }   
            else if (statusCode == HttpStatusCode.MethodNotAllowed)
            {
                string requestRoute = $"[{statusCode}]: INVALID METHOD PATH: [{context.Request.Method}]: {context.Request.Path}";
                logger.LogError(LoggerParams.None, requestRoute);
            }
            else
            {
                string requestRoute = $"[{statusCode}]: PATH: [{context.Request.Method}]: {context.Request.Path}";
                logger.LogError(LoggerParams.None, requestRoute);
            }  
        });

        await app.RunAsync(url);
    }
}
