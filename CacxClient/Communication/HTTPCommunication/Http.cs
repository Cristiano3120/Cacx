using CacxClient.Helpers;
using CacxShared;
using CacxShared.ApiResources;
using Cristiano3120.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace CacxClient.Communication.HTTPCommunication;

public sealed class Http
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
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
        => await HandleRequestAsync<T, T>(HttpRequestType.Get, default, endpoint, callerInfos);

    public async Task<ApiResponse<bool>> DeleteAsync(string endpoint, CallerInfos callerInfos)
        => await HandleRequestAsync<object, bool>(HttpRequestType.Delete, null, endpoint, callerInfos);

    public async Task<ApiResponse<TOutput>> PostAsync<TInput, TOutput>(TInput input, string endpoint, CallerInfos callerInfos)
        => await HandleRequestAsync<TInput, TOutput>(HttpRequestType.Post, input, endpoint, callerInfos);

    public async Task<ApiResponse<TOutput>> PatchAsync<TInput, TOutput>(TInput input, string endpoint, CallerInfos callerInfos)
        => await HandleRequestAsync<TInput, TOutput>(HttpRequestType.Patch, input, endpoint, callerInfos);

    public async Task<ApiResponse<TOutput>> PutAsync<TInput, TOutput>(TInput input, string endpoint, CallerInfos callerInfos)
        => await HandleRequestAsync<TInput, TOutput>(HttpRequestType.Put, input, endpoint, callerInfos);

    private async Task<ApiResponse<TOutput>> HandleRequestAsync<TInput, TOutput>(HttpRequestType httpRequestType
        , TInput? input, string endpoint, CallerInfos callerInfos)
    {
        _logger.LogInformation(LoggerParams.None, $"[{httpRequestType}]: {endpoint}");

        try
        {
            HttpResponseMessage responseMessage = httpRequestType switch
            {
                HttpRequestType.Get => await _httpClient.GetAsync(endpoint),
                HttpRequestType.Delete => await _httpClient.DeleteAsync(endpoint),
                _ => await SendDataViaHttpAsync(httpRequestType, input, endpoint)
            };

            if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError(LoggerParams.None, $"INVALID PATH: [{httpRequestType}]: {endpoint}");
                return new ApiResponse<TOutput>();
            }

            string jsonContent = await responseMessage.Content.ReadAsStringAsync();
            _logger.LogHttpPayload<TOutput>(LoggerParams.None, PayloadType.Received, httpRequestType, jsonContent);

            ApiResponse<TOutput> response = JsonSerializer.Deserialize<ApiResponse<TOutput>>(jsonContent, _jsonSerializerOptions)
                ?? throw new InvalidOperationException($"Deserialized result was null. Expected {typeof(ApiResponse<TOutput>)}");

            if (!response.IsSuccess)
            {
                _logger.LogError(LoggerParams.None, $"[{response.Error.StatusCode}]: {response.Error.Message}", callerInfos);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerParams.None, ex, callerInfos: callerInfos, calleInfos: CallerInfos.Create());
            return new ApiResponse<TOutput>();
        }
    }

    private async Task<HttpResponseMessage> SendDataViaHttpAsync<TInput>(HttpRequestType httpRequestType
        , TInput input, string endpoint)
    {
        CallerInfos callerInfos = CallerInfos.Create();
        HttpContent content;

        if (input is IMultipartFormData multipartFormData)
        {
            content = multipartFormData.ToMultipartContent();
            _logger.LogInformation(LoggerParams.None, $"[{httpRequestType}] Sending multipart/form-data to {endpoint}");
        }
        else
        {
            string jsonContent = JsonSerializer.Serialize(input, _jsonSerializerOptions);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            _logger.LogHttpPayload<TInput>(LoggerParams.None, PayloadType.Sent, httpRequestType, jsonContent);
        }

        return httpRequestType switch
        {
            HttpRequestType.Post => await _httpClient.PostAsync(endpoint, content),
            HttpRequestType.Patch => await _httpClient.PatchAsync(endpoint, content),
            HttpRequestType.Put => await _httpClient.PutAsync(endpoint, content),
            _ => throw new NotImplementedException($"{httpRequestType} is not allowed in this method " +
            $"| {callerInfos.FilePath}, line: {callerInfos.LineNum}")
        };
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
