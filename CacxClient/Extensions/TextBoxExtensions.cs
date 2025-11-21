using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CacxClient.Extensions;

public static class TextBoxExtensions
{
    public static void EnableFocusAnimation(this TextBox textBox)
    {
        _ = textBox.ApplyTemplate();
        if (textBox.Template.FindName("Border", textBox) is not Border border)
        {
            return;
        }

        textBox.Foreground = textBox.Foreground.Clone();
        border.BorderBrush = border.BorderBrush.Clone();

        textBox.GotFocus += (_, __) =>
        {
            ColorAnimation fgAnim = new()
            {
                To = (Color)Application.Current.Resources["HoverColor"],
                Duration = TimeSpan.FromSeconds(0.3)
            };
            textBox.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, fgAnim);

            ColorAnimation borderAnim = new()
            {
                To = (Color)Application.Current.Resources["HoverColor"],
                Duration = TimeSpan.FromSeconds(0.5)
            };
            border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, borderAnim);
        };

        textBox.LostFocus += (_, __) =>
        {
            ColorAnimation fgAnim = new()
            {
                To = (Color)Application.Current.Resources["TextPrimaryColor"],
                Duration = TimeSpan.FromSeconds(0.2)
            };
            textBox.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, fgAnim);

            ColorAnimation borderAnim = new()
            {
                To = (Color)Application.Current.Resources["BorderPrimary"],
                Duration = TimeSpan.FromSeconds(0.2)
            };
            border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, borderAnim);
        };
    }
}
