using CacxClient.Abstractions;
using Cristiano3120.Logging;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CacxClient.Services;

public sealed class Http : IHttp
{
    private readonly ITokenProvider _tokenProvider;
    private readonly HttpClient _httpClient;
    private readonly Logger _logger;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public Http(ITokenProvider tokenProvider, IConfiguration configuration, Logger logger)
    {
        string? uriStr = configuration.GetValue<bool>("testing")
            ? configuration.GetValue<string>("testApiBaseUrl")
            : configuration.GetValue<string>("apiBaseUrl");

        if (string.IsNullOrWhiteSpace(uriStr))
        {
            throw new InvalidOperationException("The Url is missing in the config file");
        }

        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(uriStr),
            Timeout = TimeSpan.FromSeconds(5),
        };

        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public async Task<ApiResponse<T>> GetAsync<T>(CallerInfos callerInfos, string endpoint)
        => await HandleOneWayRequestAsync<T>(callerInfos, HttpRequestType.Get, endpoint);

    public async Task<ApiResponse<bool>> DeleteAsync(CallerInfos callerInfos, string endpoint)
        => await HandleOneWayRequestAsync<bool>(callerInfos, HttpRequestType.Get, endpoint);

    public async Task<ApiResponse<TOutput>> PostAsync<TInput, TOutput>(TInput data, string endpoint, CallerInfos callerInfos)
        => await HandleTwoWayRequestAsync<TInput, TOutput>(data, HttpRequestType.Post, endpoint, callerInfos);

    public async Task<ApiResponse<TOutput>> PutAsync<TInput, TOutput>(TInput data, string endpoint, CallerInfos callerInfos)
        => await HandleTwoWayRequestAsync<TInput, TOutput>(data, HttpRequestType.Put, endpoint, callerInfos);

    private async Task<ApiResponse<T>> HandleOneWayRequestAsync<T>(CallerInfos callerInfos, HttpRequestType requestType, string endpoint)
    {
        try
        {
            _logger.LogInformation(LoggerParams.None, () => $"[{requestType}]: {endpoint}");
            using HttpRequestMessage request = new(ToHttpMethod(requestType), endpoint);

            string? token = _tokenProvider.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);
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

    public async Task<ApiResponse<TOutput>> HandleTwoWayRequestAsync<TInput, TOutput>(TInput data, HttpRequestType requestType, string endpoint, CallerInfos callerInfos)
    {
        try
        {
            _logger.LogInformation(LoggerParams.None, () => $"[{requestType}]: {endpoint}");
            using HttpRequestMessage request = new(ToHttpMethod(requestType), endpoint);

            string jsonData = JsonSerializer.Serialize(data, _jsonSerializerOptions);
            request.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            _logger.LogHttpPayload<TInput>(LoggerParams.NoNewLine, PayloadType.Sent, requestType, () => jsonData);

            string? token = _tokenProvider.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);
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
}
