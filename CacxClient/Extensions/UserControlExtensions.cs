using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows;
using Cacx.LanguageManager.Core;

namespace CacxClient.Extensions;

internal static class UserControlExtensions
{
    /// <summary>
    /// Switches the current MainWindow content to the provided UserControl with a fade animation.<br></br>
    /// Call this like: new SomeUserControl().SwitchTo();<br></br>
    /// Real example: new RegisterWindow().SwitchTo();
    /// <para>
    /// <param name="resourceBasePath"></param>
    /// </para>
    /// </summary>
    /// <param name="windowToSwitchTo"></param>
    internal static void SwitchTo(this UserControl windowToSwitchTo, string? resourceBasePath)
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

                if (resourceBasePath is not null)
                {
                    LocalizationProvider.Service.UpdateContext(resourceBasePath);
                }

                windowToSwitchTo.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };

            oldContent.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        });
    }
}
