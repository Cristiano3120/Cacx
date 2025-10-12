using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CacxClient.ExtensionMethods;

internal static class TextBlockExtensions
{
    /// <summary>
    /// Triggers an animated display of the specified message in the given TextBlock, transitioning its foreground color
    /// from the starting brush to the target color.
    /// </summary>
    /// <remarks>If an animation is already in progress, it will be cancelled before starting the new
    /// animation. This method is thread-safe and can be called multiple times to update the displayed message and
    /// animation.</remarks>
    /// <param name="textBlock">The TextBlock control in which the message will be displayed and animated.</param>
    /// <param name="startingBrush">The initial Brush used for the TextBlock's foreground before the animation begins.</param>
    /// <param name="colorToAnimateTo">The target Color to which the TextBlock's foreground will animate.</param>
    /// <param name="msg">The message text to display in the TextBlock during the animation.</param>
    /// <param name="cancellationTokenSource">A reference to a CancellationTokenSource used to manage and cancel any ongoing animation. The reference will be
    /// replaced with a new CancellationTokenSource for the current animation.</param>
    public static void TriggerDisplayAnimation(
        this TextBlock textBlock,
        Brush startingBrush,
        Color colorToAnimateTo,
        string msg,
        ref CancellationTokenSource? cancellationTokenSource)
    {
        CancellationTokenSource newCts = new();

        // atomically replace the field and get the old CTS
        CancellationTokenSource? old = Interlocked.Exchange(ref cancellationTokenSource, newCts);

        try
        {
            old?.Cancel();
        }
        catch { /* Ignore */ } 

        old?.Dispose();
        _ = Task.Run(async () 
            => await TriggerDisplayAnimationAsync(textBlock, startingBrush, colorToAnimateTo, msg, newCts.Token));
    }

    /// <summary>
    /// Animates the specified TextBlock to display a message with a foreground color transition, then hides it after a
    /// short duration.
    /// </summary>
    /// <remarks>If the operation is canceled, the TextBlock's visibility and animations are reset to their
    /// original state. The method must be called from a context that has access to the TextBlock's
    /// Dispatcher.</remarks>
    /// <param name="textBlock">The TextBlock control to animate and display the message.</param>
    /// <param name="startingBrush">The initial Brush used for the TextBlock's foreground before the animation begins.</param>
    /// <param name="colorToAnimateTo">The target Color to animate the TextBlock's foreground to during the display animation.</param>
    /// <param name="msg">The message text to display in the TextBlock during the animation.</param>
    /// <param name="cancellationToken">A CancellationToken that can be used to cancel the animation and restore the TextBlock's original state.</param>
    /// <returns>A task that represents the asynchronous operation of displaying and animating the TextBlock.</returns>
    private static async Task TriggerDisplayAnimationAsync(
        TextBlock textBlock,
        Brush startingBrush,
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
                textBlock.Foreground = startingBrush;

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
}
