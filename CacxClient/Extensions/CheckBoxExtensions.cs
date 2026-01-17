using CacxClient.Resources;
using CacxClient.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            checkMark.Stroke = BrushExtensions.PlayHoverAnimation(checkMark.Stroke);
        };

        checkBox.MouseLeave += (_, __) =>
        {
            border.PlayUnhoverAnimation();
            checkMark.Stroke = BrushExtensions.PlayUnhoverAnimation(checkMark.Stroke);
        };
    }
}
