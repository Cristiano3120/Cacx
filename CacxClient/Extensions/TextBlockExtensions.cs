using CacxClient.Helper;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CacxClient.Extensions;

internal static class TextBlockExtensions
{
    public static readonly DependencyProperty AnimationCTSProperty =
        DependencyProperty.RegisterAttached(
            name: "AnimationCTS",
            propertyType: typeof(CancellationTokenSource),
            ownerType: typeof(TextBlockExtensions),
            defaultMetadata: new PropertyMetadata(null));

    public static void SetAnimationCTS(this TextBlock textBlock, CancellationTokenSource value)
        => Application.Current.Dispatcher.Invoke(() => textBlock.SetValue(AnimationCTSProperty, value));
    
    public static CancellationTokenSource? GetAnimationCTS(this TextBlock textBlock)
    {
        CancellationTokenSource? cts = Application.Current.Dispatcher.Invoke(() =>
        {
            if (textBlock.GetValue(AnimationCTSProperty) is CancellationTokenSource cts)
            {
                return cts;
            }

            return null;
        });

        return cts;
    }

    /// <summary>
    /// Triggers an animated display of the specified message in the given TextBlock, transitioning its foreground color 
    /// to the target color.
    /// </summary>
    /// <remarks>If an animation is already in progress, it will be cancelled before starting the new
    /// animation. This method is thread-safe and can be called multiple times to update the displayed message and
    /// animation.</remarks>
    /// <param name="textBlock">The TextBlock control in which the message will be displayed and animated.</param>
    /// <param name="colorToAnimateTo">The target Color to which the TextBlock's foreground will animate.</param>
    /// <param name="msg">The message text to display in the TextBlock during the animation.</param>
    public static void TriggerDisplayAnimation(this TextBlock textBlock, Color colorToAnimateTo, string msg)
    {
        CancellationTokenSource newCts = new();
        CancellationTokenSource? oldCts = GetAnimationCTS(textBlock);
        SetAnimationCTS(textBlock, newCts);

        try
        {
            oldCts?.Cancel();
        }
        catch { /* Ignore */ }

        oldCts?.Dispose();

        _ = Task.Run(async () => await TriggerDisplayAnimationAsync(textBlock, colorToAnimateTo, msg, newCts.Token));
    }

    private static async Task TriggerDisplayAnimationAsync(
        TextBlock textBlock,
        Color colorToAnimateTo,
        string msg,
        CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken); // Give some time for the previous animation to cancel properly (NEEDED!)

        try
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(2.5);
            string originalText = string.Empty;

            await textBlock.Dispatcher.InvokeAsync(() =>
            {
                originalText = textBlock.Text;

                if (textBlock.RenderTransform is not TranslateTransform)
                {
                    textBlock.RenderTransform = new TranslateTransform();
                }

                textBlock.Text = msg;
                textBlock.Visibility = Visibility.Visible;

                Storyboard storyboard = CreateDisplayStoryboard(textBlock, colorToAnimateTo);
                storyboard.Begin(textBlock, true);
            }, DispatcherPriority.Render, cancellationToken);

            await Task.Delay(timeSpan, cancellationToken);

            await textBlock.Dispatcher.InvokeAsync(() =>
            {
                textBlock.Visibility = Visibility.Hidden;
                textBlock.Text = originalText;
            }, DispatcherPriority.Render, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            await textBlock.Dispatcher.InvokeAsync(() =>
            {
                textBlock.BeginAnimation(TextBlock.ForegroundProperty, null);
                textBlock.BeginAnimation(TextBlock.RenderTransformProperty, null);
                textBlock.Visibility = Visibility.Hidden;
            }, DispatcherPriority.Render, cancellationToken);
        }
    }

    private static Storyboard CreateDisplayStoryboard(TextBlock textBlock, Color colorToAnimateTo)
    {
        ColorAnimation colorAnimation = new()
        {
            From = GuiHelper.Darken(colorToDarken: colorToAnimateTo, factor: 0.3),
            To = colorToAnimateTo,
            Duration = TimeSpan.FromSeconds(0.35),
        };

        Storyboard.SetTarget(colorAnimation, textBlock);
        Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(path: "(Foreground).(SolidColorBrush.Color)"));

        DoubleAnimation moveAnimation = new()
        {
            From = 0,
            To = -20,
            Duration = TimeSpan.FromSeconds(0.25),
        };

        Storyboard.SetTarget(moveAnimation, textBlock);
        Storyboard.SetTargetProperty(moveAnimation, new PropertyPath(path: "(UIElement.RenderTransform).(TranslateTransform.Y)"));

        Storyboard storyboard = new();
        storyboard.Children.Add(colorAnimation);
        storyboard.Children.Add(moveAnimation);

        return storyboard;
    }

    public static void EnableMoveAnimation(this TextBlock textBlock, TextBox targetTextBox, double labelYPos, double moveBy)
    {
        TranslateTransform transform = new(offsetX: 0, offsetY: labelYPos);
        textBlock.Foreground = textBlock.Foreground.Clone();
        textBlock.RenderTransform = transform;

        targetTextBox.GotFocus += (_, __) =>
        {

            DoubleAnimation doubleAnimation = new()
            {
                Duration = TimeSpan.FromMilliseconds(200),
                By = -moveBy
            };
            textBlock.RenderTransform.BeginAnimation(TranslateTransform.YProperty, doubleAnimation);

            ColorAnimation fgAnim = new()
            {
                To = (Color)Application.Current.Resources["HoverColor"],
                Duration = TimeSpan.FromMilliseconds(200)
            };
            textBlock.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, fgAnim);

            targetTextBox.LostFocus += (_, __) =>
            {
                DoubleAnimation doubleAnimation = new()
                {
                    Duration = TimeSpan.FromMilliseconds(200),
                    By = moveBy
                };
                textBlock.RenderTransform.BeginAnimation(TranslateTransform.YProperty, doubleAnimation);

                ColorAnimation fgAnim = new()
                {
                    To = (Color)Application.Current.Resources["TextPrimaryColor"],
                    Duration = TimeSpan.FromMilliseconds(200)
                };
                textBlock.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, fgAnim);
            };
        };

    }
}
