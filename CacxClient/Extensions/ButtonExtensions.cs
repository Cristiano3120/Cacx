using System.Windows.Controls;

namespace CacxClient.Extensions;

internal static class ButtonExtensions
{
    public static void EnableHoverAnimation(this Button button)
    {
        _ = button.ApplyTemplate();

        if (button.Template.FindName("Border", button) is not Border border)
        {
            return;
        }

        border.BorderBrush = border.BorderBrush?.Clone();
        button.MouseEnter += (_, __) =>
        {
            border.PlayHoverAnimation();
            button.Foreground = button.Foreground.PlayHoverAnimation();
        };

        button.MouseLeave += (_, __) =>
        {
            border.PlayUnhoverAnimation();
            button.Foreground = button.Foreground.PlayUnhoverAnimation();
        };
    }
}
