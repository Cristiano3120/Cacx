using System.Windows.Media;

namespace CacxClient.Abstractions;

internal sealed record Theme
{
    public string? Name { get; init; } = default!;

    public IReadOnlyDictionary<string, Color> Colors { get; init; } = new Dictionary<string, Color>();
}
