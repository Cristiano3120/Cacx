using CacxClient.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CacxClient.Extensions;

internal static class CheckBoxExtensions
{
    public static void EnableHoverAnimation(this CheckBox checkBox)
    {
        _ = checkBox.ApplyTemplate();

        if (checkBox.Template.FindName("Border", checkBox) is not Border border)
        {
            return;
        }

        if (checkBox.Template.FindName("CheckMark", checkBox) is not Path checkMark)
        {
            return;
        }

        checkBox.MouseEnter += (_, __) =>
        {
            border.BorderBrush = checkBox.BorderBrush.Clone();
            checkMark.Stroke = checkMark.Stroke.Clone();

            Color? hoverColor = ThemeManager.GetColor(key: "HoverColor");
            Duration duration = new(TimeSpan.FromMilliseconds(400));

            ColorAnimation checkMarkAnimation = new()
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
            checkMark.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, checkMarkAnimation);
        };

        checkBox.MouseLeave += (_, __) =>
        {
            border.BorderBrush = checkBox.BorderBrush.Clone();

            Duration duration = new(TimeSpan.FromMilliseconds(250));
            ColorAnimation checkMarkAnimation = new()
            {
                To = ThemeManager.GetColor(key: "TextPrimaryColor"),
                Duration = duration
            };

            ColorAnimation bBAnimation = new()
            {
                To = ThemeManager.GetColor(key: "BorderPrimaryColor"),
                Duration = duration
            };

            border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, bBAnimation);
            checkMark.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, checkMarkAnimation);
        };
    }
}
