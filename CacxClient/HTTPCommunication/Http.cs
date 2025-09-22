using CacxShared.ApiResources;
using Cristiano3120.Logging;
using System.CodeDom;
using System.Net.Http;
using System.Text.Json;

namespace CacxClient.HTTPCommunication;

internal sealed class Http
{
    private JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _httpClient;
    private readonly Logger _logger;

    public Http(Logger logger)
    {
        logger.LogDebug(LoggerParams.None, "Initialized HttpClient");

        _logger = logger;
        _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(5),
            BaseAddress = new Uri(GetUri()),
        };
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, CallerInfos callerInfos)
    {

    }

    public async Task<ApiResponse<bool>> DeleteAsync(string endpoint, CallerInfos callerInfos)
    {

    }

    public async Task<ApiResponse<TOutput>> PostAsync<TInput, TOutput>(string endpoint, CallerInfos callerInfos)
    {

    }

    private async Task<ApiResponse<TOutput>> HandleRequestAsync<TInput, TOutput>(HttpRequestType httpRequestType
        , TInput input, string endpoint, CallerInfos callerInfos)
    {
        _logger.LogInformation(LoggerParams.None, $"[{httpRequestType}]: {endpoint}");

        try
        {
            HttpResponseMessage responseMessage = httpRequestType switch
            {
                HttpRequestType.Get => await _httpClient.GetAsync(endpoint),
                HttpRequestType.Delete => await _httpClient.DeleteAsync(endpoint),
                _ => throw new NotImplementedException($"The HttpRequestType you used is not implemented yet" +
                $". Used: {httpRequestType}, Called from: {callerInfos.FilePath}, {callerInfos.CallerName}(...), line: {callerInfos.LineNum}"),
            };

            string jsonContent = await responseMessage.Content.ReadAsStringAsync();
            _logger.LogHttpPayload<TOutput>(LoggerParams.None, PayloadType.Received, httpRequestType, jsonContent);

            ApiResponse<TOutput> response = JsonSerializer.Deserialize<ApiResponse<TOutput>>(jsonContent, _jsonSerializerOptions)
                ?? throw new InvalidOperationException($"Deserialized result was null. Expected {typeof(ApiResponse<TOutput>)}");
        
            if (!response.IsSuccess)
            {
                _logger.LogError(LoggerParams.None, $"[{response.Error.StatusCode}]: {response.Error.Message}" , callerInfos);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerParams.None, ex, callerInfos);
            return new ApiResponse<TOutput>();
        }
    }

    private static string GetUri()
    {
        JsonElement config = Helper.GetConfig();

        if (config.GetProperty("Testing").GetBoolean())
        {
            return config.GetProperty("TestUrl").GetString()
                ?? throw new InvalidOperationException("The TestUrl in the appSettings.json file is missing!");
        }

        return config.GetProperty("Url").GetString()
            ?? throw new InvalidOperationException("The Url in the appSettings.json file is missing!");
    }
}
