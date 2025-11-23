using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CacxClient.Helper;

public static class GuiHelper
{
    internal static void SwitchWindow(UserControl windowToSwitchTo)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Application app = Application.Current;
            Window mainWindow = app.MainWindow;

            if (mainWindow.Content is not UIElement oldContent)
            {
                mainWindow.Content = windowToSwitchTo;
                return;
            }

            TimeSpan duration = TimeSpan.FromMilliseconds(150);

            DoubleAnimation fadeOut = new(fromValue: 1, toValue: 0, duration);
            DoubleAnimation fadeIn = new(fromValue: 0, toValue: 1, duration);

            fadeOut.Completed += (_, _) =>
            {
                mainWindow.Content = windowToSwitchTo;
                windowToSwitchTo.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };

            oldContent.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        });
    }

    /// <summary>
    /// Darkens the given color by a given factor
    /// </summary>
    /// <param name="colorToDarken"></param>
    /// <param name="factor">0.0 == black; 1.0 no change</param>
    /// <returns></returns>
    public static Color Darken(Color colorToDarken, double factor)
    {
        return Color.FromRgb(
            (byte)(colorToDarken.R * factor),
            (byte)(colorToDarken.G * factor),
            (byte)(colorToDarken.B * factor));
    }
}
