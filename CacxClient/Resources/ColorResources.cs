using System.Windows;
using System.Windows.Media;

namespace CacxClient.Resources;

internal static class ColorResources
{
    public static Color TextErrorColor => (Color)Application.Current.Resources[ColorResourceKeys.TextErrorColor];
}
