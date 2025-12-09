using System.Windows.Media;

namespace CacxClient.Abstractions;

internal sealed record Theme
{
    public string? Name { get; init; } = default!;

    public Dictionary<string, Color> Colors { get; init; } = [];
}
