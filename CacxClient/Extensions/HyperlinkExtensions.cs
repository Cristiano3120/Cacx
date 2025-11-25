using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CacxClient.Extensions;

internal static class HyperlinkExtensions
{
    public static void EnableHoverAnimation(this Hyperlink hyperlink)
    {
        Duration duration = new(TimeSpan.FromMilliseconds(300));

        hyperlink.MouseEnter += (sender, args) =>
        {
            ColorAnimation colorAnimation = new()
            {
                To = (Color)Application.Current.Resources.MergedDictionaries[0]["HoverColor"],
                Duration = duration
            };

            hyperlink.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        };

        hyperlink.MouseLeave += (sender, args) =>
        {
            ColorAnimation colorAnimation = new()
            {
                To = (Color)Application.Current.Resources.MergedDictionaries[0]["TextPrimaryColor"],
                Duration = duration
            };
            
            hyperlink.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        };
    }
}
