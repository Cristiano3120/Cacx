using System.Windows.Input;

namespace CacxClient.Abstractions;

public interface ICursorService
{
    public void SetCursor(Cursor cursor);
    public void ResetCursor();
}
