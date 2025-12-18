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
            
            Color hoverColor = (Color)Application.Current.Resources["HoverColor"];
            Duration duration = new(TimeSpan.FromMilliseconds(400));

            ColorAnimation fgAnimation = new()
            {
                To = hoverColor,
                Duration = duration
            };

            ColorAnimation bBAnimation = new()
            {
                To = hoverColor,
                Duration = duration
            };

            border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, bBAnimation);
            button.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, fgAnimation);
        };

        button.MouseLeave += (_, __) =>
        {
            border.BorderBrush = button.BorderBrush.Clone();
            button.Foreground = button.Foreground.Clone();

            Duration duration = new(TimeSpan.FromMilliseconds(250));
            ColorAnimation fgAnimation = new()
            {
                To = (Color)Application.Current.Resources["TextPrimaryColor"],
                Duration = duration
            };

            ColorAnimation bBAnimation = new()
            {
                To = (Color)Application.Current.Resources["BorderPrimaryColor"],
                Duration = duration
            };

            border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, bBAnimation);
            button.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, fgAnimation);
        };
    }
}
