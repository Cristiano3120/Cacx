using System.Windows.Media;

namespace CacxClient.Extensions;

internal static class ColorExtensions
{
    /// <summary>
    /// Darkens the given color by a given factor
    /// </summary>
    /// <param name="colorToDarken"></param>
    /// <param name="factor">0.0 == black; 1.0 no change</param>
    /// <returns></returns>
    public static Color Darken(this Color colorToDarken, double factor)
    {
        return Color.FromRgb(
            (byte)(colorToDarken.R * factor),
            (byte)(colorToDarken.G * factor),
            (byte)(colorToDarken.B * factor));
    }
}
