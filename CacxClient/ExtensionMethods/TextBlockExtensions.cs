using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CacxClient.ExtensionMethods;

internal static class TextBlockExtensions
{
    public static async Task TriggerDisplayAnimationAsync(this TextBlock textBlock,Brush startingBrush,
        Color colorToAnimateTo, string msg,
        CancellationToken cancellationToken)
    {
        try
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(2.5);
            string originalText = textBlock.Text;
            textBlock.Foreground = startingBrush;

            if (textBlock.RenderTransform is not TranslateTransform)
            {
                TranslateTransform transform = new();
                textBlock.RenderTransform = transform;
            }

            textBlock.Text = msg;
            textBlock.Visibility = Visibility.Visible;
            textBlock.BeginStoryboard(CreateDisplayStoryboard(textBlock, colorToAnimateTo));

            await Task.Delay(timeSpan, cancellationToken);

            textBlock.Visibility = Visibility.Hidden;
            textBlock.Text = originalText;
        }
        catch (TaskCanceledException)
        {
            // Ignore
        }
    }

    private static Storyboard CreateDisplayStoryboard(TextBlock textBlock, Color colorToAnimateTo)
    { 
        ColorAnimation colorAnimation = new()
        {
            To = colorToAnimateTo,
            Duration = TimeSpan.FromSeconds(0.35),
        };

        Storyboard.SetTarget(colorAnimation, textBlock);
        Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Foreground).(SolidColorBrush.Color)"));

        DoubleAnimation moveAnimation = new()
        {
            From = 0,
            To = -20,
            Duration = TimeSpan.FromSeconds(0.25),
        };

        Storyboard.SetTarget(moveAnimation, textBlock);
        Storyboard.SetTargetProperty(moveAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
    
        Storyboard storyboard = new();
        storyboard.Children.Add(colorAnimation);
        storyboard.Children.Add(moveAnimation);

        return storyboard;
    }
}
