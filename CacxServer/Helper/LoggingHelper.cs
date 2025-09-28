using CacxShared.ApiResources;
using System.Text.Json;

namespace CacxServer.Helper;

public sealed class LoggingHelper
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public LoggingHelper(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    /// <summary>
    /// This method puts infos into the HttpContext so we can log more precisely in the <see cref="GlobalActionFilter"/>
    /// </summary>
    public void AddInfosForLogging<T>(HttpContext httpContext, Type type, ApiResponse<T> apiResponse)
    {
        httpContext.Items["Type"] = type;
        httpContext.Items["ApiResponseStr"] = JsonSerializer.Serialize(apiResponse, _jsonSerializerOptions);
    }
}
