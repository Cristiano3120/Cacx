using System.Windows.Controls;
using System.Windows.Shapes;

namespace CacxClient.Extensions;

internal static class CheckBoxExtensions
{
    public static void EnableHoverAnimation(this CheckBox checkBox)
    {
        _ = checkBox.ApplyTemplate();
        
        if (checkBox.Template.FindName("Border", checkBox) is not Border border)
        {
            return;
        }

        if (checkBox.Template.FindName("CheckMark", checkBox) is not Path checkMark)
        {
            return;
        }

        checkBox.MouseEnter += (_, __) =>
        {
            border.PlayHoverAnimation();
            checkMark.Stroke = checkMark.Stroke.PlayHoverAnimation();
        };

        checkBox.MouseLeave += (_, __) =>
        {
            border.PlayUnhoverAnimation();
            checkMark.Stroke = checkMark.Stroke.PlayUnhoverAnimation();
        };
    }
}
