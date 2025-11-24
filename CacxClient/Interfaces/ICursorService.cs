using System.Windows.Input;

namespace CacxClient.Interfaces;

public interface ICursorService
{
    public void SetCursor(Cursor cursor);
    public void ResetCursor();
}
