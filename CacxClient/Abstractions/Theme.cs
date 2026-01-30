using System.Windows.Media;

namespace CacxClient.Abstractions;

public sealed record Theme(string Name, IReadOnlyDictionary<string, Color> Colors);