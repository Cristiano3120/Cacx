using CacxClient.Communication.HTTPCommunication;
using CacxShared.ApiResources;
using CacxShared.Endpoints;
using Cristiano3120.Logging;

namespace CacxClient.Communication;

internal sealed class ConnectionHandler
{
    private readonly Logger _logger;
    private readonly Http _http;

    public ConnectionHandler(Logger logger, Http http)
    {
        _logger = logger;
        _http = http;
    }

    public async Task CheckServerStatusAsync()
    {
        string endpoint = Endpoints.GetAuthEndpoint(AuthEndpoint.Ping);
        CallerInfos callerInfos = CallerInfos.Create();
        int msWaitDelay = 3000;

        while (true)
        {
            try
            {
                _ = await _http.GetAsync<bool>(endpoint, callerInfos);
                _logger.LogInformation(LoggerParams.None, "Ping successful!");
                return;
            }
            catch (Exception)
            {
                _logger.LogWarning(LoggerParams.None, "Server not reachable at the moment!");
                await Task.Delay(msWaitDelay);
            }
        }
    }
}
