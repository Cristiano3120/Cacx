using CacxClient.Resources;
using CacxClient.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

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

        button.MouseEnter += (_, __) =>
        {
            border.BorderBrush = button.BorderBrush.Clone();
            button.Foreground = button.Foreground.Clone();
            
            Duration duration = new(TimeSpan.FromMilliseconds(400));

            ColorAnimation fgAnimation = new()
            {
                To = ColorResources.HoverColor,
                Duration = duration
            };

            ColorAnimation bBAnimation = new()
            {
                To = ColorResources.HoverColor,
                Duration = duration
            };

            border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, bBAnimation);
            button.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, fgAnimation);
        };

        button.MouseLeave += (_, __) =>
        {
            border.BorderBrush = button.BorderBrush.Clone();

            Duration duration = new(TimeSpan.FromMilliseconds(250));
            ColorAnimation fgAnimation = new()
            {
                To = ColorResources.TextPrimaryColor,
                Duration = duration
            };

            ColorAnimation bBAnimation = new()
            {
                To = ColorResources.BorderPrimaryColor,
                Duration = duration
            };

            border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, bBAnimation);
            button.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, fgAnimation);
        };
    }
}
