using CacxClient.Resources;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CacxClient.Extensions;

internal static class BrushExtensions
{
    public static Brush PlayHoverAnimation(this Brush brush)
    {
        if (brush is not SolidColorBrush originalBrush)
            return brush;

        SolidColorBrush brushtoAnimate = new(originalBrush.Color);
        ColorAnimation fgAnim = new()
        {
            To = ColorResources.HoverColor,
            Duration = TimeSpan.FromSeconds(0.5)
        };

        brushtoAnimate.BeginAnimation(SolidColorBrush.ColorProperty, fgAnim);

        return brushtoAnimate;
    }

    public static Brush PlayUnhoverAnimation(this Brush brush)
    {
        if (brush is not SolidColorBrush originalBrush)
            return brush;

        SolidColorBrush brushtoAnimate = new(originalBrush.Color);
        ColorAnimation fgAnim = new()
        {
            To = ColorResources.TextPrimaryColor,
            Duration = TimeSpan.FromSeconds(0.3)
        };

        brushtoAnimate.BeginAnimation(SolidColorBrush.ColorProperty, fgAnim);

        return brushtoAnimate;
    }
}
