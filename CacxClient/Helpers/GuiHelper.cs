using CacxClient.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CacxClient.Helpers;

internal static class GuiHelper
{
    /// <summary>
    /// Displays the specified window, replacing the currently active window in the application.
    /// </summary>
    /// <remarks>Use this method to switch the application's main interface to a different window. The
    /// previously active window will be hidden or closed as appropriate. This method is typically used in navigation
    /// scenarios where only one window should be visible at a time.</remarks>
    /// <param name="windowToShow">The window to display. Must not be null.</param>
    public static void SwitchWindow<TNewWindow>(TNewWindow windowToShow) where TNewWindow : UserControl
        => Internal_SwitchWindow(windowToShow);

    /// <summary>
    /// Closes all windows and creates a new <see cref="Window"/> of type <typeparamref name="TNewWindow"/>
    /// </summary>
    /// <typeparam name="TNewWindow">The type of <see cref="Window"/> that should be instantiated</typeparam>
    /// <param name="newWindow">
    /// If the <see cref="Window"/> needs a special constructor that needs to be called
    /// instantiate a <see cref="Window"/> of that type beforehand and just pass it as an param
    /// </param>
    public static void SwitchWindow<TNewWindow>() where TNewWindow : UserControl, new()
        => Internal_SwitchWindow(new TNewWindow());

    private static void Internal_SwitchWindow<TNewWindow>(TNewWindow newWindow) where TNewWindow : UserControl
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Application app = Application.Current;
            Window mainWindow = app.MainWindow;

            if (mainWindow.Content is not UIElement oldContent)
            {
                mainWindow.Content = newWindow;
                return;
            }

            TimeSpan duration = TimeSpan.FromMilliseconds(150);

            DoubleAnimation fadeOut = new(fromValue: 1, toValue: 0, duration);
            DoubleAnimation fadeIn = new(fromValue: 0, toValue: 1, duration);

            fadeOut.Completed += (_, _) =>
            {
                mainWindow.Content = newWindow;
                newWindow.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };

            oldContent.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        });
    }

    /// <summary>
    /// This method deactivates the button and shows a loading animation(loading cursor)
    /// </summary>
    /// <param name="requestDone">If <c>true</c> the UI changes will be reverted</param>
    internal static void ChangeRequestUI(UserControl userControl, Button btn, bool requestDone)
    {
        btn.IsEnabled = requestDone;
        userControl.Cursor = requestDone 
            ? Cursors.Arrow 
            : Cursors.Wait;
    }
}
