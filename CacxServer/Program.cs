using CacxShared;
using Cristiano3120.Logging;

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

        string url = (builder.Configuration.GetValue<bool>(key: "Testing")
            ? builder.Configuration.GetValue<string>("TestUrl")
            : builder.Configuration.GetValue<string>("Url")) 
            ?? throw new InvalidOperationException("The appSettings.json file is broken");

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            _ = app.MapOpenApi();
        }

        _ = app.UseHttpsRedirection();
        _ = app.UseAuthorization(); 
        _ = app.MapControllers();

        app.Run(url);
    }
}
