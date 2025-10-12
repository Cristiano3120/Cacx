using CacxClient.Communication.HTTPCommunication;
using Cristiano3120.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CacxClient;

public partial class App : Application
{

    /// <summary>
    /// Retrieves the application's configured logger instance for logging operations.
    /// </summary>
    /// <remarks>The returned logger is resolved from the application's dependency injection container. The
    /// method throws an exception if the logger service is not registered.</remarks>
    /// <returns>A <see cref="Logger"/> instance used for logging messages and events within the application.</returns>
    public static Logger GetLogger() => _serviceProvider.GetRequiredService<Logger>();

    /// <summary>
    /// Retrieves an instance of the <see cref="Http"/> service from the application's service provider.
    /// </summary>
    /// <remarks>Throws an exception if the <see cref="Http"/> service is not registered with the service
    /// provider.</remarks>
    /// <returns>An <see cref="Http"/> instance resolved from the service provider.</returns>
    public static Http GetHttp() => _serviceProvider.GetRequiredService<Http>();
}
