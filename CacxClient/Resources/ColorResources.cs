using System.Windows;
using System.Windows.Media;

namespace CacxClient.Resources;

internal static class ColorResources
{
    public static Color TextErrorColor => (Color)Application.Current.Resources[ColorResourceKeys.TextErrorColor];
    public static Color TextPrimaryColor => (Color)Application.Current.Resources[ColorResourceKeys.TextPrimaryColor];
    public static Color HoverColor => (Color)Application.Current.Resources[ColorResourceKeys.HoverColor];
    public static Color BorderPrimaryColor => (Color)Application.Current.Resources[ColorResourceKeys.BorderPrimaryColor];
}
