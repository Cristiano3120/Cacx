using CacxShared.Helper;
using Cristiano3120.Logging;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CacxServer;

internal class GlobalActionFilter(Logger logger) :  IActionFilter
{
    private readonly Logger _logger = logger;

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (!context.HttpContext.Items.TryGetValue("Type", out object? typeObj) 
            || !context.HttpContext.Items.TryGetValue("ApiResponseStr", out object? apiResponseObjStr)
            || apiResponseObjStr is not string apiResponseStr)
        {
            _logger.LogError(LoggerParams.None, "The <Type>/<ApiResponse> key was missing in the dict");
            return;
        }

        Type type = typeObj!.GetType();
        HttpRequestType requestType = Enum.Parse<HttpRequestType>(context.HttpContext.Request.Method.FromUpperToNormal());

        _logger.LogHttpPayload(type, LoggerParams.None, PayloadType.Sent, requestType, apiResponseStr);
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        string requestRoute = $"[{context.HttpContext.Request.Method}]: {context.HttpContext.Request.Path}";
        _logger.LogWarning(LoggerParams.NoNewLine, requestRoute);
    }
}