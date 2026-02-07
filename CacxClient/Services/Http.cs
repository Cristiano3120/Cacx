using CacxClient.Abstractions;
using CacxShared.Abstractions;
using Cristiano3120.Logging;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CacxClient.Services;

public sealed class Http : IHttp
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IDeviceIDProvider _deviceIDProvider;
    private readonly ITokenProvider _tokenProvider;
    private readonly HttpClient _httpClient;
    private readonly Logger _logger;


    public Http(
        JsonSerializerOptions serializerOptions, 
        IDeviceIDProvider deviceIDProvider,
        ITokenProvider tokenProvider, 
        IConfiguration configuration, 
        Logger logger)
    {
        string? uriStr = configuration.GetValue<bool>(key: "testing")
            ? configuration.GetValue<string>(key: "testApiBaseUrl")
            : configuration.GetValue<string>(key: "apiBaseUrl");

        if (string.IsNullOrWhiteSpace(uriStr))
        {
            throw new InvalidOperationException("The Url is missing in the config file");
        }

        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(uriStr),
            Timeout = TimeSpan.FromSeconds(5),
        };

        _jsonSerializerOptions = serializerOptions;
        _deviceIDProvider = deviceIDProvider;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, CallerInfos callerInfos)
        => await HandleOneWayRequestAsync<T>(requestType: HttpRequestType.Get, callerInfos, endpoint);

    public async Task<ApiResponse<bool>> DeleteAsync(string endpoint, CallerInfos callerInfos)
        => await HandleOneWayRequestAsync<bool>(requestType: HttpRequestType.Delete, callerInfos, endpoint);

    public async Task<ApiResponse<TOutput>> PostAsync<TInput, TOutput>(TInput data, string endpoint, CallerInfos callerInfos)
        => await HandleTwoWayRequestAsync<TInput, TOutput>(data, HttpRequestType.Post, endpoint, callerInfos);

    public async Task<ApiResponse<TOutput>> PutAsync<TInput, TOutput>(TInput data, string endpoint, CallerInfos callerInfos)
        => await HandleTwoWayRequestAsync<TInput, TOutput>(data, HttpRequestType.Put, endpoint, callerInfos);

    private async Task<ApiResponse<T>> HandleOneWayRequestAsync<T>(
        HttpRequestType requestType, 
        CallerInfos callerInfos, 
        string endpoint)
    {
        try
        {
            _logger.LogInformation(LoggerParams.None, () => $"[{requestType}]: {endpoint}");
            using HttpRequestMessage request = new(ToHttpMethod(requestType), endpoint);

            AddHeaders(request);

            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogHttpPayload<T>(LoggerParams.NoNewLine, PayloadType.Received, requestType, () => responseContent);

            ApiResponse<T> body = JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, _jsonSerializerOptions)!;
            return ApiResponse<T>.FromHttp(body, headers: response.Headers);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerParams.None, ex, callerInfos);
            return ApiResponse<T>.Error(HttpStatusCode.InternalServerError, "Can´t reach the server at the moment try again!");
        }
    }

    public async Task<ApiResponse<TOutput>> HandleTwoWayRequestAsync<TInput, TOutput>(
        TInput data, 
        HttpRequestType requestType, 
        string endpoint, 
        CallerInfos callerInfos)
    {
        try
        {
            _logger.LogInformation(LoggerParams.None, () => $"[{requestType}]: {endpoint}");
            using HttpRequestMessage request = new(ToHttpMethod(requestType), endpoint);

            string jsonData = JsonSerializer.Serialize(data, _jsonSerializerOptions);
            request.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

            _logger.LogHttpPayload<TInput>(LoggerParams.NoNewLine, PayloadType.Sent, requestType, () => jsonData);

            AddHeaders(request);

            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogHttpPayload<TOutput>(LoggerParams.NoNewLine, PayloadType.Received, requestType, () => responseContent);
            
            ApiResponse<TOutput> body = JsonSerializer.Deserialize<ApiResponse<TOutput>>(responseContent, _jsonSerializerOptions)!;
            return ApiResponse<TOutput>.FromHttp(body, headers: response.Headers);
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerParams.None, ex, callerInfos);
            return ApiResponse<TOutput>.Error(HttpStatusCode.InternalServerError, "Can´t reach the server at the moment try again!");
        }
    }

    private static HttpMethod ToHttpMethod(HttpRequestType type)
    {
        return type switch
        {
            HttpRequestType.Get => HttpMethod.Get,
            HttpRequestType.Delete => HttpMethod.Delete,
            HttpRequestType.Post => HttpMethod.Post,
            HttpRequestType.Put => HttpMethod.Put,
            _ => throw new NotSupportedException($"HTTP method '{type}' is not supported.")
        };
    }

    private void AddHeaders(HttpRequestMessage request)
    {
        string? token = _tokenProvider.GetToken();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Add(AuthHeaderNames.AuthTokenHeader, token);
        }

        Guid deviceId = _deviceIDProvider.GetDeviceID();
        request.Headers.Add(AuthHeaderNames.DeviceIdHeader, deviceId.ToString());
    }
}