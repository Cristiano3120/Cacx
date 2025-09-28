using CacxServer.Helper;
using CacxShared.ApiResources;
using CacxShared.Helper;
using Cristiano3120.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace CacxServer;

public static class Program
{
    public static void Main(string[] args)
    {
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
            PathToLogDirectory = SharedHelper.GetDynamicPath("Logs")
        };
        Logger logger = new(loggerSettings);
        _ = builder.Services.AddSingleton(logger);

        //Get the Url depending on the environment
        string url = (builder.Configuration.GetValue<bool>(key: "Testing")
            ? builder.Configuration.GetValue<string>("TestUrl")
            : builder.Configuration.GetValue<string>("Url"))
            ?? throw new InvalidOperationException("The appSettings.json file is broken");

        //Configure JsonSerializerOptions
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        _ = builder.Services.AddSingleton(jsonSerializerOptions);

        _ = builder.Services.AddSingleton<LoggingHelper>();

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

            //Path not found!
            if (statusCode == HttpStatusCode.NotFound)
            {
                string requestRoute = $"INVALID PATH: [{context.Request.Method}]: {context.Request.Path}";
                logger.LogError(LoggerParams.None, requestRoute);
            }
        });

        app.Run(url);
    }
}
