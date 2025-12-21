using CacxShared.Abstractions;
using CacxShared.Services;
using Cristiano3120.Logging;
using LogLevel = Cristiano3120.Logging.LogLevel;

namespace CacxServer;

public class Program
{
    protected Program() { }

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        PathProvider pathProvider = new();

        _ = builder.Services.AddSingleton<IPathProvider, PathProvider>((_) => pathProvider);

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

