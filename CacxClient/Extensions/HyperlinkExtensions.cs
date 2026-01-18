using System.Windows.Documents;

namespace CacxClient.Extensions;

internal static class HyperlinkExtensions
{
    public static void EnableHoverAnimation(this Hyperlink hyperlink)
    {
        hyperlink.MouseEnter += (_, _) => hyperlink.Foreground = hyperlink.Foreground.PlayHoverAnimation();
        hyperlink.MouseLeave += (_, _) => hyperlink.Foreground = hyperlink.Foreground.PlayUnhoverAnimation();
    }
}
