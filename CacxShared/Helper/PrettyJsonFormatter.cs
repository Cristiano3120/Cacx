using Serilog.Events;
using Serilog.Formatting;
using System.Text.Json;

namespace CacxShared.Helper;

public class PrettyJsonFormatter : ITextFormatter
{
    readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public void Format(LogEvent logEvent, TextWriter output)
    {
        var json = new
        {
            Timestamp = logEvent.Timestamp,
            Level = logEvent.Level.ToString(),
            Message = logEvent.RenderMessage(),
            Exception = logEvent.Exception?.ToString(),
            Properties = logEvent.Properties
        };

        string formatted = JsonSerializer.Serialize(json, _jsonSerializerOptions);

        output.WriteLine(formatted);
    }
}
