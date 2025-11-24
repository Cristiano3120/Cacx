using CacxClient.Interfaces;
using System.Windows;
using System.Windows.Input;

namespace CacxClient.Services;

public class CursorService : ICursorService
{
    public void SetCursor(Cursor cursor)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Mouse.OverrideCursor = cursor;
        });
    }

    public void ResetCursor()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Mouse.OverrideCursor = null;
        });
    }
}
