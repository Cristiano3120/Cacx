using CacxClient.Resources;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CacxClient.Extensions;

internal static class BorderExtensions
{
    public static void PlayHoverAnimation(this Border border)
    {
        if (border.BorderBrush is null)
            return;

        ColorAnimation borderAnim = new()
        {
            To = ColorResources.HoverColor,
            Duration = TimeSpan.FromSeconds(0.5)
        };
        border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, borderAnim);
    }

    public static void PlayUnhoverAnimation(this Border border)
    {
        if (border.BorderBrush is null)
            return;

        ColorAnimation borderAnim = new()
        {
            To = ColorResources.BorderPrimaryColor,
            Duration = TimeSpan.FromSeconds(0.3)
        };
        border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, borderAnim);
    }
}
