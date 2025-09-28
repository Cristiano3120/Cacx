using System.Windows;

namespace CacxClient.Helpers;

internal static class GuiHelper
{
    /// <summary>
    /// Closes all windows and creates a new <see cref="Window"/> of type <typeparamref name="TNewWindow"/>
    /// </summary>
    /// <typeparam name="TNewWindow">The type of <see cref="Window"/> that should be instantiated</typeparam>
    /// <param name="newWindow">
    /// If the <see cref="Window"/> needs a special constructor that needs to be called
    /// instantiate a <see cref="Window"/> of that type beforehand and just pass it as an param
    /// </param>
    public static void SwitchWindow<TNewWindow>(TNewWindow? newWindow = null) where TNewWindow : Window, new()
    {
        //The delay before closing the old window in ms
        const int SwitchDelayMs = 150;

        _ = Application.Current.Dispatcher.Invoke(async() =>
        {
            newWindow ??= new TNewWindow();
            newWindow.Show();

            await Task.Delay(SwitchDelayMs);

            foreach (Window window in Application.Current.Windows)
            {
                if (window is not TNewWindow)
                {
                    window.Close();
                    break;
                }
            }
        });
    }
}
